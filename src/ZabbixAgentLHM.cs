using LibreHardwareMonitor.Hardware;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class ZabbixAgentLHM
{
    private List<Item> items;
    private List<SensorType> sensorTypes;
    private List<ComputerHardwareType> hardwareTypes;

    private Export export;
    private string prefix;

    // Constructors

    public ZabbixAgentLHM(
        List<SensorType> sensorTypes,
        List<ComputerHardwareType> hardwareTypes,
        string prefix
        )
    {
        this.items = new List<Item>();
        this.export = new Export();
        this.sensorTypes = sensorTypes;
        this.hardwareTypes = hardwareTypes;
        this.prefix = prefix;

        Template template = new Template();
        this.export.AddTemplate(template);
    }

    public ZabbixAgentLHM(
        List<SensorType> sensorTypes,
        List<ComputerHardwareType> hardwareTypes,
        string prefix,
        string templateName,
        string templateGroupName
        )
    {
        this.items = new List<Item>();
        this.export = new Export();
        this.sensorTypes = sensorTypes;
        this.hardwareTypes = hardwareTypes;
        this.prefix = prefix;

        Group group = new Group(templateGroupName);
        Group nameOnly = new Group(templateGroupName, false);

        Template template = new Template(templateName);
        template.AddGroup(nameOnly);

        this.export.AddGroup(group);
        this.export.AddTemplate(template);
    }

    public void PrintYaml()
    {
        this.export.GetTemplate().AddItem(Utilities.NewMasterItem(this.prefix));

        var exportDict = new Dictionary<string, Export>();
        exportDict.Add("zabbix_export", this.export);

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

        foreach (Item item in this.export.GetTemplate().Items) {
            if (item.Key != null && item.Value != null) {
                sensorDict.Add(item.Key, item.Value ?? 0);
            }
        }

        string jsonString = JsonSerializer.Serialize(sensorDict);
        Console.WriteLine(jsonString);
    }

    private void processHardware(IHardware hardware)
    {
        foreach (ISensor sensor in hardware.Sensors)
        {
            if (this.sensorTypes.Contains(sensor.SensorType)) {

                Item item = new Item(
                    this.prefix,
                    hardware.Name,
                    sensor.Name,
                    sensor.SensorType,
                    sensor.Value
                );
                item.AddMasterItem("lhm.gather");
                item.Delay = 0;

                this.export.GetTemplate().AddItem(item);
            }
        }
    }

    public void Gather(IVisitor visitor)
    {
        Computer computer = new Computer
        {
            IsBatteryEnabled = this.hardwareTypes.Contains(ComputerHardwareType.Battery),
            IsControllerEnabled = this.hardwareTypes.Contains(ComputerHardwareType.Controller),
            IsCpuEnabled = this.hardwareTypes.Contains(ComputerHardwareType.Cpu),
            IsGpuEnabled = this.hardwareTypes.Contains(ComputerHardwareType.Gpu),
            IsMemoryEnabled = this.hardwareTypes.Contains(ComputerHardwareType.Memory),
            IsMotherboardEnabled = this.hardwareTypes.Contains(ComputerHardwareType.Motherboard),
            IsNetworkEnabled = this.hardwareTypes.Contains(ComputerHardwareType.Network),
            IsPsuEnabled = this.hardwareTypes.Contains(ComputerHardwareType.Psu),
            IsStorageEnabled = this.hardwareTypes.Contains(ComputerHardwareType.Storage),
        };

        computer.Open();
        computer.Accept(visitor);

        foreach (IHardware hardware in computer.Hardware)
        {
            foreach (IHardware subhardware in hardware.SubHardware)
            {
                this.processHardware(subhardware);
            }

            this.processHardware(hardware);
        }

        computer.Close();
    }

}

