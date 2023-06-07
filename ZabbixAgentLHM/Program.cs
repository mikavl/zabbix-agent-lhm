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
    public const string DefaultTemplateName = "Template App LibreHardwareMonitor";

    public const string DefaultTemplateGroupName = "Templates/LibreHardwareMonitor";



    public static async Task Main(string[] args)
    {
        Option<string> prefixOption = NewStringOptionWithAlias(
            "--prefix",
            "-p",
            "Prefix to use for Zabbix keys",
            "lhm");

        Option<string> sensorTypesOption = NewStringOptionWithAlias(
            "--sensor-types",
            "-s",
            "Comma separated list of sensor types to gather",
            "fan,power,temperature");

        Option<string> hardwareTypesOption = NewStringOptionWithAlias(
            "--hardware-types",
            "-t",
            "Comma separated list of hardware types to gather",
            "cpu,motherboard,storage");

        Option<string> outputOption = NewStringOptionWithAlias(
            "--output",
            "-o",
            "Save Zabbix template to this file instead of stdout",
            null);

        Option<string> templateGroupOption = NewStringOptionWithAlias(
            "--template-group",
            "-g",
            "Name of the Zabbix template group",
            DefaultTemplateGroupName);

        Option<string> templateNameOption = NewStringOptionWithAlias(
            "--template-name",
            "-n",
            "Name of the Zabbix template",
            DefaultTemplateName);

        Command gatherCommand = NewCommandWithStringOptions(
            "gather",
            "Gather sensor data",
            new Option<string>[] {
                prefixOption,
                sensorTypesOption,
                hardwareTypesOption
            });

        Command templateCommand = NewCommandWithStringOptions(
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

        RootCommand rootCommand = new RootCommand(
            "Zabbix agent integration with LibreHardwareMonitor");

        rootCommand.AddCommand(gatherCommand);
        rootCommand.AddCommand(templateCommand);

        await rootCommand.InvokeAsync(args);
    }

    public static Option<string> NewStringOptionWithAlias(
        string name,
        string alias,
        string description,
        string? defaultValue)
    {
        var option = new Option<string>(
            name: name,
            description: description);

        option.AddAlias(alias);

        if (defaultValue is string defaultValueNotNull)
        {
            option.SetDefaultValue(defaultValueNotNull);
        }

        return option;
    }

    public static Command NewCommandWithStringOptions(
        string name,
        string description,
        Option<string>[] options)
    {
        var command = new Command(name, description);

        foreach (Option<string> option in options)
        {
            command.Add(option);
        }

        return command;
    }

    public static SensorType[] SensorTypesFromString(string sensorTypesString)
    {
        return GenericTypesFromString<SensorType>(sensorTypesString);
    }

    public static ComputerHardwareType[] ComputerHardwareTypesFromString(string computerHardwareTypesString)
    {
        return GenericTypesFromString<ComputerHardwareType>(computerHardwareTypesString);
    }

    public static T[] GenericTypesFromString<T>(string typesString) where T : struct, System.Enum
    {
        T type;
        var types = new List<T>();

        foreach (var typeString in typesString.Split(","))
        {
            if (typeString.ToLower().Equals("all"))
            {
                foreach (T t in Enum.GetValues(typeof(T)))
                {
                    types.Add(t);
                }
            }
            else if (Enum.TryParse<T>(typeString, true, out type))
            {
                types.Add(type);
            }
            else
            {
                throw new System.Exception($"Unknown type {typeString}");
            }
        }

        return types.ToArray();
    }

    static void Gather(string prefix, string hardwareTypesString, string sensorTypesString)
    {
        SensorType[] sensorTypes = SensorTypesFromString(sensorTypesString);
        ComputerHardwareType[] computerHardwareTypes = ComputerHardwareTypesFromString(hardwareTypesString);

        var visitor = new Visitor(
            prefix,
            computerHardwareTypes,
            sensorTypes,
            // The following are not really used in the gather command, so pass defaults
            DefaultTemplateName,
            DefaultTemplateGroupName
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
        SensorType[] sensorTypes = SensorTypesFromString(sensorTypesString);
        ComputerHardwareType[] computerHardwareTypes = ComputerHardwareTypesFromString(hardwareTypesString);
        var visitor = new Visitor(
            prefix,
            computerHardwareTypes,
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


