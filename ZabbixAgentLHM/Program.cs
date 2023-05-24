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

public class Program
{
    public static async Task Main(string[] args)
    {
        Option<string> prefixOption = NewOptionWithAlias(
            "--prefix",
            "-p",
            "Prefix to use for Zabbix keys",
            "lhm");

        Option<string> sensorTypesOption = NewOptionWithAlias(
            "--sensor-types",
            "-s",
            "Comma separated list of sensor types to gather",
            "fan,power,temperature");

        Option<string> hardwareTypesOption = NewOptionWithAlias(
            "--hardware-types",
            "-t",
            "Comma separated list of hardware types to gather",
            "cpu,motherboard,storage");

        Option<string> outputOption = NewOptionWithAlias(
            "--output",
            "-o",
            "Save Zabbix template to this file instead of stdout",
            null);

        Option<string> templateGroupOption = NewOptionWithAlias(
            "--template-group",
            "-g",
            "Name of the Zabbix template group",
            "Templates/LibreHardwareMonitor");

        Option<string> templateNameOption = NewOptionWithAlias(
            "--template-name",
            "-n",
            "Name of the Zabbix template",
            "Template App LibreHardwareMonitor");

        var gatherCommand = NewCommandWithOptions(
            "gather",
            "Gather sensor data",
            new Option<string>[] {
                prefixOption,
                sensorTypesOption,
                hardwareTypesOption
            });

        var templateCommand = NewCommandWithOptions(
            "template",
            "Write Zabbix template",
            new Option<string>[] {
                prefixOption,
                sensorTypesOption,
                hardwareTypesOption,
                outputOption,
                templateGroupOption,
                templateNameOption
            });

        gatherCommand.SetHandler((
            prefixOptionValue,
            hardwareTypesOptionValue,
            sensorTypesOptionValue) => Gather(
                prefixOptionValue,
                hardwareTypesOptionValue,
                sensorTypesOptionValue),
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
            templateNameOptionValue) => Template(
                prefixOptionValue,
                hardwareTypesOptionValue,
                sensorTypesOptionValue,
                outputOptionValue,
                templateGroupOptionValue,
                templateNameOptionValue),
            prefixOption,
            hardwareTypesOption,
            sensorTypesOption,
            outputOption,
            templateGroupOption,
            templateNameOption
        );

        var rootCommand = new RootCommand(
            "Zabbix agent integration with LibreHardwareMonitor");

        rootCommand.AddCommand(gatherCommand);
        rootCommand.AddCommand(templateCommand);

        await rootCommand.InvokeAsync(args);
    }

    private static Option<string> NewOptionWithAlias(
        string Name,
        string Alias,
        string Description,
        string? DefaultValue)
    {
        var option = new Option<string>(
                        name: Name,
                        description: Description);

        option.AddAlias(Alias);

        if (DefaultValue is string defaultValue)
        {
                option.SetDefaultValue(defaultValue);
        }

        return option;
    }

    private static Command NewCommandWithOptions(
        string Name,
        string Description,
        Option<string>[] Options)
    {
        var command = new Command(Name, Description);

        foreach (Option<string> option in Options)
        {
                command.Add(option);
        }

        return command;
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

