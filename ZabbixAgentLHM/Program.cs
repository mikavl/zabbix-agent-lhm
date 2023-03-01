using LibreHardwareMonitor.Hardware;

using System.Text;
using System.Text.Json;
using System;
using System.Linq;
using System.CommandLine;
using System.Security.Cryptography;


using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ZabbixAgentLHM;

class Program
{
    static async Task Main(string[] args)
    {
        var rootCommand = AddCommands();
        await rootCommand.InvokeAsync(args);
    }

    static RootCommand AddCommands()
    {
        //
        // Command definitions
        //

        var rootCommand = new RootCommand("Zabbix agent integration with LibreHardwareMonitor");
        var gatherCommand = new Command("gather", "Gather sensor data");
        var templateCommand = new Command("template", "Write Zabbix template");

        //
        // Common command options
        //

        var prefixOption = new Option<string>(
            name: "--prefix",
            description: "Prefix to use for Zabbix keys",
            getDefaultValue: () => "lhm"
        );
        prefixOption.AddAlias("-p");

        var sensorTypesOption = new Option<string>(
            name: "--sensor-types",
            description: "Comma separated list of sensor types to gather",
            getDefaultValue: () => "fan,power,temperature"
        );
        sensorTypesOption.AddAlias("-s");

        var hardwareTypesOption = new Option<string>(
            name: "--hardware-types",
            description: "Comma separated list of hardware types to gather",
            getDefaultValue: () => "cpu,motherboard,storage"
        );
        hardwareTypesOption.AddAlias("-t");

        //
        // Template command options
        //

        var outputOption = new Option<string>(
            name: "--output",
            description: "Save Zabbix template to this file instead of stdout"
        );
        outputOption.AddAlias("-o");

        var templateGroupOption = new Option<string>(
            name: "--template-group",
            description: "Name of the Zabbix template group",
            getDefaultValue: () => "Templates/LibreHardwareMonitor"
        );
        templateGroupOption.AddAlias("-g");

        var templateNameOption = new Option<string>(
            name: "--template-name",
            description: "Name of the Zabbix template",
            getDefaultValue: () => "Template App LibreHardwareMonitor"
        );
        templateNameOption.AddAlias("-n");

        //
        // Associate commands and options
        //

        templateCommand.Add(outputOption);
        templateCommand.Add(prefixOption);
        templateCommand.Add(sensorTypesOption);
        templateCommand.Add(hardwareTypesOption);
        templateCommand.Add(templateGroupOption);
        templateCommand.Add(templateNameOption);

        gatherCommand.Add(prefixOption);
        gatherCommand.Add(sensorTypesOption);
        gatherCommand.Add(hardwareTypesOption);

        rootCommand.Add(gatherCommand);
        rootCommand.Add(templateCommand);

        //
        // Command handlers
        //

        gatherCommand.SetHandler((
                prefixOptionValue,
                hardwareTypesOptionValue,
                sensorTypesOptionValue
            ) => Gather(
                prefixOptionValue,
                hardwareTypesOptionValue,
                sensorTypesOptionValue
            ),
            prefixOption,
            hardwareTypesOption,
            sensorTypesOption
        );

        templateCommand.SetHandler((
                prefixOptionValue,
                hardwareTypesOptionValue,
                sensorTypesOptionValue,
                outputOptionValue,
                templateGroupOptionValue,
                templateNameOptionValue
            ) => Template(
                prefixOptionValue,
                hardwareTypesOptionValue,
                sensorTypesOptionValue,
                outputOptionValue,
                templateGroupOptionValue,
                templateNameOptionValue
            ),
            prefixOption,
            hardwareTypesOption,
            sensorTypesOption,
            outputOption,
            templateGroupOption,
            templateNameOption
        );

        return rootCommand;
    }


    static void Gather(string prefix, string hardwareTypesString, string sensorTypesString)
    {
        var sensorTypes = Utilities.ParseSensorTypes(sensorTypesString);
        var hardwareTypes = Utilities.ParseHardwareTypes(hardwareTypesString);
        var visitor = new Visitor(
            prefix,
            hardwareTypes,
            sensorTypes
        );

        visitor.Gather();

        IDictionary<string, float> sensorDict = new Dictionary<string, float>();

        foreach(var item in visitor.Export.Templates.First().Items)
        {
            if (!String.IsNullOrEmpty(item.Key))
            {
                sensorDict.Add(item.Key, item.Value ?? 0);
            }
        }


        var options = new JsonSerializerOptions
        {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
        };

        Console.WriteLine(JsonSerializer.Serialize(sensorDict, options));
    }

    static void Template(
        string prefix,
        string hardwareTypesString,
        string sensorTypesString,
        string output,
        string templateGroupName,
        string templateName)
    {
        var sensorTypes = Utilities.ParseSensorTypes(sensorTypesString);
        var hardwareTypes = Utilities.ParseHardwareTypes(hardwareTypesString);
        var visitor = new Visitor(
            prefix,
            hardwareTypes,
            sensorTypes,
            templateName,
            templateGroupName
        );

        visitor.Gather();

        var serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitEmptyCollections | DefaultValuesHandling.OmitNull)
            .Build();

        if (String.IsNullOrEmpty(output))
        {
            serializer.Serialize(Console.Out, visitor);
        }
        else
        {
            using var writer = new System.IO.StreamWriter(output);
            serializer.Serialize(writer, visitor);
        }
    }

}

