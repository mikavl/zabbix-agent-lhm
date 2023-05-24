using LibreHardwareMonitor.Hardware;
using YamlDotNet.Serialization;

namespace ZabbixAgentLHM;

public class Visitor : IVisitor
{
    [YamlMember(Alias = "zabbix_export")]
    public Export Export { get; }

    private ComputerHardwareType[] _hardwareTypes;

    private string _prefix { get; }

    private SensorType[] _sensorTypes;

    public Visitor(
        string prefix,
        ComputerHardwareType[] hardwareTypes,
        SensorType[] sensorTypes)
    {
        this._prefix = prefix;
        this._hardwareTypes = hardwareTypes;
        this._sensorTypes = sensorTypes;

        this.Export = new Export();
    }

    public Visitor(
        string prefix,
        ComputerHardwareType[] hardwareTypes,
        SensorType[] sensorTypes,
        string templateName,
        string templateGroupName) : this(prefix, hardwareTypes, sensorTypes)
    {
        var template = new Template();
        template.SetName(templateName);
        template.SetGroup(templateGroupName);

        this.Export.SetGroup(templateGroupName);
        this.Export.SetTemplate(template);
    }

    public void Gather()
    {
        var hwTypes = this._hardwareTypes;
        var computer = new Computer
        {
            IsBatteryEnabled = hwTypes.Contains(ZabbixAgentLHM.ComputerHardwareType.Battery),
            IsControllerEnabled = hwTypes.Contains(ZabbixAgentLHM.ComputerHardwareType.Controller),
            IsCpuEnabled = hwTypes.Contains(ZabbixAgentLHM.ComputerHardwareType.Cpu),
            IsGpuEnabled = hwTypes.Contains(ZabbixAgentLHM.ComputerHardwareType.Gpu),
            IsMemoryEnabled = hwTypes.Contains(ZabbixAgentLHM.ComputerHardwareType.Memory),
            IsMotherboardEnabled = hwTypes.Contains(ZabbixAgentLHM.ComputerHardwareType.Motherboard),
            IsNetworkEnabled = hwTypes.Contains(ZabbixAgentLHM.ComputerHardwareType.Network),
            IsPsuEnabled = hwTypes.Contains(ZabbixAgentLHM.ComputerHardwareType.Psu),
            IsStorageEnabled = hwTypes.Contains(ZabbixAgentLHM.ComputerHardwareType.Storage),
        };

        computer.Open();
        computer.Accept(this);
        computer.Close();

        // Add the master item
        var masterItem = new Item();

        masterItem.Key = $"{this._prefix}.gather";
        masterItem.Name = "LibreHardwareMonitor";
        masterItem.History = 0;
        masterItem.Trends = 0;
        masterItem.ValueType = "TEXT";
        masterItem.Type = "ZABBIX_ACTIVE";

        this.Export.Templates.First().Items.Add(masterItem);
    }

    public void ProcessHardware(IHardware hardware)
    {
        foreach (ISensor sensor in hardware.Sensors)
        {
            if (this._sensorTypes.Contains(sensor.SensorType))
            {
                var item = new Item();

                item.Key = Utilities.ItemKey(this._prefix, sensor.Identifier);
                item.Name = Utilities.ItemName(hardware.Name, sensor.Name);
                item.Value = sensor.Value;
                item.Units = Utilities.Units(sensor.SensorType);
                item.Delay = 0;
                item.SetMasterItem("lhm.gather");
                item.Preprocessing.Add(Utilities.NewDefaultPreprocessor(item.Key));
                item.Tags.Add(new Tag("Component", Utilities.ComponentName(hardware.HardwareType)));

                this.Export.Templates.First().Items.Add(item);
            }
        }
    }

    public void VisitComputer(IComputer computer)
    {
        computer.Traverse(this);
    }

    public void VisitHardware(IHardware hardware)
    {
        hardware.Update();
        this.ProcessHardware(hardware);

        foreach (IHardware subHardware in hardware.SubHardware)
        {
            subHardware.Accept(this);
        }
    }

    public void VisitParameter(IParameter parameter) { }

    public void VisitSensor(ISensor sensor) { }
}
