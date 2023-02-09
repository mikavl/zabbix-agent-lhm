using LibreHardwareMonitor.Hardware;

using System.Text;
using System;
using System.CommandLine;
using System.Security.Cryptography;

namespace ZabbixAgentLHM
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand("Zabbix agent integration with LibreHardwareMonitor");
            var gatherCommand = new Command("gather", "Gather sensor data");
            var templateCommand = new Command("template", "Write Zabbix template");

            var outputOption = new Option<string>(
                name: "--output",
                description: "Save template to this file, or stdout if not specified"
            );

            templateCommand.Add(outputOption);

            rootCommand.Add(gatherCommand);
            rootCommand.Add(templateCommand);


            gatherCommand.SetHandler(() => Gather());
            templateCommand.SetHandler((outputOptionValue) => Template(outputOptionValue), outputOption);

            await rootCommand.InvokeAsync(args);
        }

        static void Gather()
        {
            List<SensorType> sensorTypes = new List<SensorType>()
            {
                SensorType.Power,
                SensorType.Temperature,
                SensorType.Fan
            };

            ZabbixAgentLHM zal = new ZabbixAgentLHM();
            Visitor v = new Visitor();

            zal.Gather(v, sensorTypes);
            zal.PrintJson();
        }

        static void Template(string output)
        {
            List<SensorType> sensorTypes = new List<SensorType>()
            {
                SensorType.Power,
                SensorType.Temperature,
                SensorType.Fan
            };

            ZabbixAgentLHM zal = new ZabbixAgentLHM();
            Visitor v = new Visitor();

            zal.Gather(v, sensorTypes);
            zal.PrintYaml();
        }

    }


    // https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/migrate-from-newtonsoft?pivots=dotnet-6-0#conditionally-ignore-a-property



}
