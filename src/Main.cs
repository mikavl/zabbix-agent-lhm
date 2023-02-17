using LibreHardwareMonitor.Hardware;

using System.Text;
using System;
using System.Linq;
using System.CommandLine;
using System.Security.Cryptography;

// HardwareType does not directly correspond to what we want to pass, so use a new enum
public enum ComputerHardwareType {
    Battery,
    Controller,
    Cpu,
    Gpu,
    Memory,
    Motherboard,
    Network,
    Psu,
    Storage
};

class Program
{
    static async Task Main(string[] args)
    {
        var rootCommand = new RootCommand("Zabbix agent integration with LibreHardwareMonitor");
        var gatherCommand = new Command("gather", "Gather sensor data");
        var templateCommand = new Command("template", "Write Zabbix template");

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


        gatherCommand.SetHandler((
                prefixOptionValue,
                sensorTypesOptionValue,
                hardwareTypesOptionValue
            ) => Gather(
                prefixOptionValue,
                sensorTypesOptionValue,
                hardwareTypesOptionValue
            ),
            prefixOption,
            sensorTypesOption,
            hardwareTypesOption
        );

        templateCommand.SetHandler((
                outputOptionValue,
                prefixOptionValue,
                sensorTypesOptionValue,
                hardwareTypesOptionValue,
                templateGroupOptionValue,
                templateNameOptionValue
            ) => Template(
                outputOptionValue,
                prefixOptionValue,
                sensorTypesOptionValue,
                hardwareTypesOptionValue,
                templateGroupOptionValue,
                templateNameOptionValue
            ),
            outputOption,
            prefixOption,
            sensorTypesOption,
            hardwareTypesOption,
            templateGroupOption,
            templateNameOption
        );

        await rootCommand.InvokeAsync(args);
    }

    static List<SensorType> ParseSensorTypes(string s)
    {
        SensorType sensorTypeEnum;
        List<SensorType> sensorTypes = new List<SensorType>();

        foreach (var sensorType in s.Split(",")) {
            if (Enum.TryParse<SensorType>(sensorType, true, out sensorTypeEnum)) {
                sensorTypes.Add(sensorTypeEnum);
            } else {
                throw new System.Exception($"Unknown sensor type {sensorType}");
            }
        }

        return sensorTypes;
    }


    static List<ComputerHardwareType> ParseHardwareTypes(string s)
    {
        ComputerHardwareType hardwareTypeEnum;
        List<ComputerHardwareType> hardwareTypes = new List<ComputerHardwareType>();

        foreach (var hardwareType in s.Split(",")) {
            if (Enum.TryParse<ComputerHardwareType>(hardwareType, true, out hardwareTypeEnum)) {
                hardwareTypes.Add(hardwareTypeEnum);
            } else {
                throw new System.Exception($"Unknown hardware type {hardwareType}");
            }
        }

        return hardwareTypes;
    }

    static void Gather(string prefix, string sensorTypes, string hardwareTypes)
    {
        List<SensorType> sts = ParseSensorTypes(sensorTypes);
        List<ComputerHardwareType> hws = ParseHardwareTypes(hardwareTypes);

        ZabbixAgentLHM zal = new ZabbixAgentLHM(sts, hws, prefix);
        Visitor v = new Visitor();

        zal.Gather(v);
        zal.PrintJson();
    }

    static void Template(
        string output,
        string prefix,
        string sensorTypes,
        string hardwareTypes,
        string templateGroupName,
        string templateName)
    {
        List<SensorType> sts = ParseSensorTypes(sensorTypes);
        List<ComputerHardwareType> hws = ParseHardwareTypes(hardwareTypes);

        ZabbixAgentLHM zal = new ZabbixAgentLHM(sts, hws, prefix, templateGroupName, templateName);
        Visitor v = new Visitor();

        zal.Gather(v);
        zal.PrintYaml();
    }

}
