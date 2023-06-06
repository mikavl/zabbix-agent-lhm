using LibreHardwareMonitor.Hardware;
using YamlDotNet.Serialization;

namespace ZabbixAgentLHM;

public class Visitor : IVisitor
{
    [YamlMember(Alias = "zabbix_export")]
    public Export Export { get; }

    private ComputerHardwareType[] _computerHardwareTypes;

    private string _prefix { get; }

    private SensorType[] _sensorTypes;

    public Visitor(
        string prefix,
        ComputerHardwareType[] computerHardwareTypes,
        SensorType[] sensorTypes)
    {
        this._prefix = prefix;
        this._computerHardwareTypes = computerHardwareTypes;
        this._sensorTypes = sensorTypes;

        this.Export = new Export();
    }

    public Visitor(
        string prefix,
        ComputerHardwareType[] computerHardwareTypes,
        SensorType[] sensorTypes,
        string templateName,
        string templateGroupName) : this(prefix, computerHardwareTypes, sensorTypes)
    {
        var template = new Template();
        template.SetName(templateName);
        template.SetGroupByName(templateGroupName);

        this.Export.SetGroupByName(templateGroupName);
        this.Export.SetTemplate(template);
    }

    public void Gather()
    {
        this.Export.GetTemplate().SetMasterItemByNameAndKey("LibreHardwareMonitor", $"{this._prefix}.gather");

        var computer = new Computer
        {
            IsBatteryEnabled     = this._computerHardwareTypes.Contains(ComputerHardwareType.Battery),
            IsControllerEnabled  = this._computerHardwareTypes.Contains(ComputerHardwareType.Controller),
            IsCpuEnabled         = this._computerHardwareTypes.Contains(ComputerHardwareType.Cpu),
            IsGpuEnabled         = this._computerHardwareTypes.Contains(ComputerHardwareType.Gpu),
            IsMemoryEnabled      = this._computerHardwareTypes.Contains(ComputerHardwareType.Memory),
            IsMotherboardEnabled = this._computerHardwareTypes.Contains(ComputerHardwareType.Motherboard),
            IsNetworkEnabled     = this._computerHardwareTypes.Contains(ComputerHardwareType.Network),
            IsPsuEnabled         = this._computerHardwareTypes.Contains(ComputerHardwareType.Psu),
            IsStorageEnabled     = this._computerHardwareTypes.Contains(ComputerHardwareType.Storage),
        };

        computer.Open();
        computer.Accept(this);
        computer.Close();
    }

    public void ProcessHardware(IHardware hardware)
    {
        Item? masterItem = this.Export.GetTemplate().MasterItem;

        foreach (ISensor sensor in hardware.Sensors)
        {
            if (this._sensorTypes.Contains(sensor.SensorType))
            {
                var item = new Item();

                item.SetKey(this._prefix, sensor.Identifier);
                item.SetName(hardware.Name, sensor.Name);
                item.Value = sensor.Value;
                item.SetUnits(sensor.SensorType);
                item.Delay = 0;
                item.AddPreprocessor(new DefaultPreprocessor(item.Key)); // TODO: fix nullable item.Key
                item.AddTag(new ComponentTag(hardware.HardwareType));

                if (masterItem is Item m)
                {
                    item.SetMasterItem(m);
                }
                else
                {
                    throw new System.Exception("Master item was null, this should not happen");
                }

                this.Export.GetTemplate().AddItem(item);
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
