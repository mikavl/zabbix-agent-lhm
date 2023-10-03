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
    public const string DefaultHardwareTypes = "cpu,motherboard,storage";

    public const string DefaultSensorTypes = "fan,power,temperature";

    public const string DefaultOutput = ""; // stdout

    public const string Description = "Zabbix agent integration with LibreHardwareMonitor";

    public enum Commands
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
            "Comma separated list of sensor types to gather, or \"all\"",
            DefaultSensorTypes);

        Option<string> hardwareTypesOption = NewStringOptionWithAlias(
            "--hardware-types",
            "-t",
            "Comma separated list of hardware types to gather, or \"all\"",
            DefaultHardwareTypes);

        Option<string> outputOption = NewStringOptionWithAlias(
            "--output",
            "-o",
            "Save Zabbix template to this file instead of stdout",
            null);

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
                outputOption
            });

        gatherCommand.SetHandler((
            hardwareTypesOptionValue,
            sensorTypesOptionValue) => Execute(
                Commands.Gather,
                hardwareTypesOptionValue,
                sensorTypesOptionValue,
                DefaultOutput),
            hardwareTypesOption,
            sensorTypesOption
        );

        templateCommand.SetHandler((
            hardwareTypesOptionValue,
            sensorTypesOptionValue,
            outputOptionValue) => Execute(
                Commands.Template,
                hardwareTypesOptionValue,
                sensorTypesOptionValue,
                outputOptionValue),
            hardwareTypesOption,
            sensorTypesOption,
            outputOption
        );

        RootCommand rootCommand = new RootCommand(Description);

        rootCommand.AddCommand(gatherCommand);
        rootCommand.AddCommand(templateCommand);

        await rootCommand.InvokeAsync(args);
    }

    static void Execute(
        Commands command,
        string hardwareTypesString,
        string sensorTypesString,
        string output)
    {
        SensorType[] sensorTypes = TypesFromString<SensorType>(sensorTypesString);
        Utilities.ComputerHardware[] hardwareTypes = TypesFromString<Utilities.ComputerHardware>(hardwareTypesString);

        var data = new Data(hardwareTypes, sensorTypes);

        data.Gather();

        switch (command)
        {
            case Commands.Gather:
                data.ToJson(Console.OpenStandardOutput());
                break;
            case Commands.Template:
                using (var writer = String.IsNullOrEmpty(output)
                    ? new System.IO.StreamWriter(Console.OpenStandardOutput())
                    : new System.IO.StreamWriter(output))
                {
                    data.ToYaml(writer);
                }
                break;
            default:
                throw new Exception($"Unhandled command {command.ToString()}");
        }
    }
}

