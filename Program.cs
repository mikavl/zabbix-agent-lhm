using LibreHardwareMonitor.Hardware;
using System;

namespace ZabbixAgentLHM
{
    public class Program
    {
        static void Main(string[] args)
        {
            ZabbixAgentLHM zal = new ZabbixAgentLHM();
            UpdateVisitor uv = new UpdateVisitor();

            zal.Open();
            zal.Gather(uv);
            zal.Close();
        }

    }

    public class ZabbixAgentLHM
    {

        private Computer computer;

        public ZabbixAgentLHM()
        {
            this.computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = false, // Use nvidia-smi for this
                IsMemoryEnabled = false, // Use zabbix-agent directly for this
                IsMotherboardEnabled = true,
                IsControllerEnabled = false, // I don't own the hardware
                IsNetworkEnabled = false, // Use zabbix-agent directly for this
                IsStorageEnabled = true,
            };

        }

        private void IterateHardware(IHardware hardware)
        {
            foreach (IHardware subhardware in hardware.SubHardware)
            {
                this.IterateHardware(subhardware);
            }

            foreach(ISensor sensor in hardware.Sensors)
            {
                Console.WriteLine("{0},{1},{2},{3},{4}", hardware.HardwareType.ToString(), hardware.Name.ToString(), sensor.Name.ToString(), sensor.Value.ToString(), sensor.SensorType.ToString());
            }
        }

        public void Gather(IVisitor visitor)
        {
            this.computer.Accept(visitor);

            foreach (IHardware hardware in this.computer.Hardware)
            {
                this.IterateHardware(hardware);
            }

        }

        public void Open()
        {
            this.computer.Open();
        }

        public void Close()
        {
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
