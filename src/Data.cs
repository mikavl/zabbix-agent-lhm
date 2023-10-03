using LibreHardwareMonitor.Hardware;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ZabbixAgentLHM;

public class Data
{
    [YamlIgnore]
    public const string GroupName = "Templates/LibreHardwareMonitor";

    [YamlMember(Alias = "ZabbixExport")]
    public Zabbix.Export Export { get; }

    private Utilities.ComputerHardware[] hardwareTypes;

    private SensorType[] sensorTypes;

    public Data(
        Utilities.ComputerHardware[] hardwareTypes,
        SensorType[] sensorTypes)
    {
        this.hardwareTypes = hardwareTypes;
        this.sensorTypes = sensorTypes;
        this.Export = new Zabbix.Export(Data.GroupName);
    }

    public void Gather()
    {
        var computer = new Computer
        {
            IsBatteryEnabled = this.hardwareTypes.Contains(Utilities.ComputerHardware.Battery),
            IsControllerEnabled = this.hardwareTypes.Contains(Utilities.ComputerHardware.Controller),
            IsCpuEnabled = this.hardwareTypes.Contains(Utilities.ComputerHardware.Cpu),
            IsGpuEnabled = this.hardwareTypes.Contains(Utilities.ComputerHardware.Gpu),
            IsMemoryEnabled = this.hardwareTypes.Contains(Utilities.ComputerHardware.Memory),
            IsMotherboardEnabled = this.hardwareTypes.Contains(Utilities.ComputerHardware.Motherboard),
            IsNetworkEnabled = this.hardwareTypes.Contains(Utilities.ComputerHardware.Network),
            IsPsuEnabled = this.hardwareTypes.Contains(Utilities.ComputerHardware.Psu),
            IsStorageEnabled = this.hardwareTypes.Contains(Utilities.ComputerHardware.Storage),
        };

        computer.Open();
        computer.Accept(new Visitor());

        var masterItem = new Zabbix.MasterItem();
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

    private void processHardware(IHardware hardware, Zabbix.IItem masterItem)
    {
        foreach (ISensor sensor in hardware.Sensors)
        {
            if (this.sensorTypes.Contains(sensor.SensorType))
            {
                var item = new Zabbix.Item(hardware, sensor);
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

