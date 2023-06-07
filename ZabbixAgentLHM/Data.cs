using LibreHardwareMonitor.Hardware;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ZabbixAgentLHM
{
    public class Data
    {
        [YamlMember(Alias = "ZabbixExport")]
        public Export Export { get; } = new Export();

        private ComputerHardware[] hardwareTypes;

        private SensorType[] sensorTypes;

        public Data(
            ComputerHardware[] hardwareTypes,
            SensorType[] sensorTypes,
            string templateName,
            string groupName)
        {
            this.hardwareTypes = hardwareTypes;
            this.sensorTypes = sensorTypes;

            var template = new Template(templateName);
            template.SetGroup(new TemplateGroup(groupName));

            this.Export.SetTemplate(template);
            this.Export.SetGroup(new ExportGroup(groupName));
        }

        public void Gather()
        {
            var computer = new Computer
            {
                IsBatteryEnabled = this.hardwareTypes.Contains(ComputerHardware.Battery),
                IsControllerEnabled = this.hardwareTypes.Contains(ComputerHardware.Controller),
                IsCpuEnabled = this.hardwareTypes.Contains(ComputerHardware.Cpu),
                IsGpuEnabled = this.hardwareTypes.Contains(ComputerHardware.Gpu),
                IsMemoryEnabled = this.hardwareTypes.Contains(ComputerHardware.Memory),
                IsMotherboardEnabled = this.hardwareTypes.Contains(ComputerHardware.Motherboard),
                IsNetworkEnabled = this.hardwareTypes.Contains(ComputerHardware.Network),
                IsPsuEnabled = this.hardwareTypes.Contains(ComputerHardware.Psu),
                IsStorageEnabled = this.hardwareTypes.Contains(ComputerHardware.Storage),
            };

            computer.Open();
            computer.Accept(new Visitor());

            var masterItem = new MasterItem();
            this.Export.AddItem(masterItem);

            foreach (IHardware hardware in computer.Hardware)
            {
                foreach (IHardware subhardware in hardware.SubHardware)
                {
                    this.processHardware(subhardware, masterItem);
                }

                this.processHardware(hardware, masterItem);
            }

            computer.Close();
        }

        private void processHardware(IHardware hardware, IItem masterItem)
        {
            foreach (ISensor sensor in hardware.Sensors)
            {
                if (this.sensorTypes.Contains(sensor.SensorType))
                {
                    var item = new Item(hardware, sensor);
                    item.SetMasterItem(masterItem);
                    this.Export.AddItem(item);
                }
            }
        }

        public void ToYaml(System.IO.StreamWriter output)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitEmptyCollections | DefaultValuesHandling.OmitNull)
                .Build();

            serializer.Serialize(output, this);
        }

        public void ToJson(System.IO.Stream output)
        {
            IDictionary<string, float> sensorDict = new Dictionary<string, float>();

            foreach(var item in this.Export.GetTemplate().Items)
            {
                sensorDict.Add(item.Key, item.Value ?? 0);
            }

            var options = new JsonSerializerOptions
            {
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            JsonSerializer.Serialize(output, sensorDict, options);
        }
    }
}
