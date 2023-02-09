using LibreHardwareMonitor.Hardware;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ZabbixAgentLHM
{

    public class ZabbixAgentLHM
    {
        private List<Item> items;

        // Prefix all Zabbix template keys, so they will be <prefix>.<hardwarew>.<sensor>
        private const string prefix = "lhm";

        public ZabbixAgentLHM()
        {
            this.items = new List<Item>();
        }

        private void AddMasterItem()
        {
            Item item = new Item();
            item.Name = "LibreHardwareMonitor";
            item.Key = "lhm.gather";
            item.History = 0;
            item.Trends = 0;
            item.ValueType = "TEXT";
            item.Type = "ZABBIX_ACTIVE";
            item.Uuid = Utilities.NewUuid();
            this.items.Add(item);
        }

        public void PrintYaml()
        {
            this.AddMasterItem();

            string groupName = "Templates/LibreHardwareMonitor";
            Group group = new Group();
            group.Name = groupName;
            group.Uuid = Utilities.NewUuid();
            Group groupNameOnly = new Group();
            groupNameOnly.Name = groupName;

            string templateName = "Template App LibreHardwareMonitor";
            Template template = new Template();
            template.Name = templateName;
            template.TemplateName = templateName;
            template.Uuid = Utilities.NewUuid();
            template.Groups.Add(groupNameOnly);
            template.Items = this.items;

            ZabbixExport zabbixExport = new ZabbixExport();
            zabbixExport.Version = "6.0";
            zabbixExport.Date = $"{DateTime.UtcNow.ToString("s")}Z";
            zabbixExport.Groups.Add(group);
            zabbixExport.Templates.Add(template);

            var exportDict = new Dictionary<string, ZabbixExport>();
            exportDict.Add("zabbix_export", zabbixExport);

            var serializer = new SerializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitEmptyCollections | DefaultValuesHandling.OmitNull)
                .Build();
            var yaml = serializer.Serialize(exportDict);
            Console.WriteLine(yaml);
        }

        public void PrintJson()
        {
            IDictionary<string, float> sensorDict = new Dictionary<string, float>();

            foreach (Item item in this.items) {
                if (item.Key != null && item.Value != null) {
                    sensorDict.Add(item.Key, item.Value ?? 0);
                }
            }

            string jsonString = JsonSerializer.Serialize(sensorDict);
            Console.WriteLine(jsonString);
        }

        private void processHardware(IHardware hardware, List<SensorType> sensorTypes)
        {
            foreach (ISensor sensor in hardware.Sensors)
            {
                if (sensorTypes.Contains(sensor.SensorType)) {

                    Item item = new Item();
                    item.SetName(hardware.Name, sensor.Name);
                    item.SetKey("lhm", hardware.Name, sensor.Name);
                    item.SetUnits(sensor.SensorType);
                    item.AddDefaultPreprocessor();
                    item.SetMasterItem("lhm.gather");
                    item.Type = "DEPENDENT";
                    item.Value = sensor.Value;
                    item.ValueType = "FLOAT";
                    item.Uuid = Utilities.NewUuid();
                    item.Delay = 0;

                    this.items.Add(item);
                }
            }
        }

        public void Gather(IVisitor visitor, List<SensorType> sensorTypes)
        {
            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsMotherboardEnabled = true,
                IsStorageEnabled = true,
            };

            computer.Open();
            computer.Accept(visitor);

            foreach (IHardware hardware in computer.Hardware)
            {
                foreach (IHardware subhardware in hardware.SubHardware)
                {
                    this.processHardware(subhardware, sensorTypes);
                }

                this.processHardware(hardware, sensorTypes);
            }

            computer.Close();
        }

    }
}
