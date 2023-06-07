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

    public const string DefaultGroupName = "Templates/LibreHardwareMonitor";

    public const string DefaultOutput = ""; // stdout

    private enum Commands
    {
        Gather,
        Template
    }

    public static T[] TypesFromString<T>(string typesString) where T : struct, System.Enum
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

    public static async Task Main(string[] args)
    {
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

        Option<string> groupNameOption = NewStringOptionWithAlias(
            "--group-name",
            "-g",
            "Name of the Zabbix template group",
            DefaultGroupName);

        Option<string> templateNameOption = NewStringOptionWithAlias(
            "--template-name",
            "-n",
            "Name of the Zabbix template",
            DefaultTemplateName);

        Command gatherCommand = NewCommandWithStringOptions(
            "gather",
            "Gather sensor data",
            new Option<string>[] {
                sensorTypesOption,
                hardwareTypesOption
            });

        Command templateCommand = NewCommandWithStringOptions(
            "template",
            "Write Zabbix template",
            new Option<string>[] {
                sensorTypesOption,
                hardwareTypesOption,
                templateNameOption,
                groupNameOption,
                outputOption
            });

        gatherCommand.SetHandler((
            hardwareTypesOptionValue,
            sensorTypesOptionValue) => Execute(
                Commands.Gather,
                hardwareTypesOptionValue,
                sensorTypesOptionValue,
                DefaultTemplateName,
                DefaultGroupName,
                DefaultOutput),
            hardwareTypesOption,
            sensorTypesOption
        );

        templateCommand.SetHandler((
            hardwareTypesOptionValue,
            sensorTypesOptionValue,
            templateNameOptionValue,
            groupNameOptionValue,
            outputOptionValue) => Execute(
                Commands.Template,
                hardwareTypesOptionValue,
                sensorTypesOptionValue,
                templateNameOptionValue,
                groupNameOptionValue,
                outputOptionValue),
            hardwareTypesOption,
            sensorTypesOption,
            templateNameOption,
            groupNameOption,
            outputOption
        );

        RootCommand rootCommand = new RootCommand("Zabbix agent integration with LibreHardwareMonitor");

        rootCommand.AddCommand(gatherCommand);
        rootCommand.AddCommand(templateCommand);

        await rootCommand.InvokeAsync(args);
    }

    static void Execute(
        Commands command,
        string hardwareTypesString,
        string sensorTypesString,
        string templateName,
        string groupName,
        string output)
    {
        SensorType[] sensorTypes = TypesFromString<SensorType>(sensorTypesString);
        ComputerHardware[] hardwareTypes = TypesFromString<ComputerHardware>(hardwareTypesString);

        var visitor = new Visitor(
            hardwareTypes,
            sensorTypes,
            templateName,
            groupName);

        if (command == Commands.Gather)
        {
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
        else if (command == Commands.Template)
        {
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
        else
        {
            throw new Exception($"Unhandled command {command.ToString()}");
        }

    }

}


