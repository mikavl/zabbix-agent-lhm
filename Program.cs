using LibreHardwareMonitor.Hardware;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZabbixAgentLHM
{
    public class Program
    {
        static void Main(string[] args)
        {
            ZabbixAgentLHM zal = new ZabbixAgentLHM();
            UpdateVisitor uv = new UpdateVisitor();

            zal.Gather(uv);
            zal.PrintJson();
        }

    }

    // https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/migrate-from-newtonsoft?pivots=dotnet-6-0#conditionally-ignore-a-property
    public class HardwareHack
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("sensors")]
        public List<SensorHack> Sensors { get; }

        public HardwareHack(string name, HardwareType type)
        {
            this.Name = name;
            this.Sensors = new List<SensorHack>();
        }
    }

    public class SensorHack
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public float? Value { get; set; }

        public SensorHack(string name, float? sensorValue)
        {
            this.Name = name;
            this.Value = sensorValue;
        }
    }

    public class ZabbixAgentLHM
    {
        private Computer computer;

        private List<HardwareHack> hardware;
        private List<SensorType> sensorTypes;

        public ZabbixAgentLHM()
        {
            this.computer = new Computer
            {
                IsCpuEnabled = true,
                IsMotherboardEnabled = true,
                IsStorageEnabled = true,
            };
            this.hardware = new List<HardwareHack>();
            this.sensorTypes = new List<SensorType>()
            {
                SensorType.Power,
                SensorType.Temperature,
                SensorType.Fan
            };
        }

        public void PrintJson()
        {
            string jsonString = JsonSerializer.Serialize(this.hardware);

            Console.WriteLine("{{\"hardware\":{0}}}", jsonString);
        }

        private void processHardware(IHardware hardware)
        {
            HardwareHack h = new HardwareHack(hardware.Name, hardware.HardwareType);

            foreach (ISensor sensor in hardware.Sensors)
            {
                if (this.sensorTypes.Contains(sensor.SensorType)) {
                    h.Sensors.Add(new SensorHack(sensor.Name, sensor.Value));
                }
            }

            if (h.Sensors.Count > 0) {
                this.hardware.Add(h);
            }
        }


        public void Gather(IVisitor visitor)
        {
            this.computer.Open();

            this.computer.Accept(visitor);

            foreach (IHardware hardware in this.computer.Hardware)
            {
                foreach (IHardware subhardware in hardware.SubHardware)
                {
                    this.processHardware(subhardware);
                }

                this.processHardware(hardware);
            }

            this.computer.Close();
        }

    }

    public class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) {
                subHardware.Accept(this);
            }
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }
}
