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

    private IItem _masterItem = new MasterItem();

    public Visitor(
        string prefix,
        ComputerHardwareType[] computerHardwareTypes,
        SensorType[] sensorTypes,
        string templateName,
        string templateGroupName)
    {
        this._prefix = prefix;
        this._computerHardwareTypes = computerHardwareTypes;
        this._sensorTypes = sensorTypes;

        this.Export = new Export();

        var template = new Template(templateName);
        template.SetGroup(new TemplateGroup(templateGroupName));

        this.Export.SetGroup(new ExportGroup(templateGroupName));
        this.Export.SetTemplate(template);
        this.AddItem(this._masterItem);
    }

    public void AddItem(IItem item)
    {
        this.Export.AddItem(item);
    }

    public void Gather()
    {
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
        foreach (ISensor sensor in hardware.Sensors)
        {
            if (this._sensorTypes.Contains(sensor.SensorType))
            {
                var item = new Item(hardware, sensor);
                item.SetMasterItem(this._masterItem);
                this.AddItem(item);
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
