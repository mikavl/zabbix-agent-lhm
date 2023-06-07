using LibreHardwareMonitor.Hardware;
using YamlDotNet.Serialization;

namespace ZabbixAgentLHM;

public class Visitor : IVisitor
{
    public Export Export { get; }

    private ComputerHardware[] computerHardware;

    private SensorType[] sensorTypes;

    private IItem masterItem = new MasterItem();

    public Visitor(
        ComputerHardware[] computerHardware,
        SensorType[] sensorTypes,
        string templateName,
        string groupName)
    {
        this.computerHardware = computerHardware;
        this.sensorTypes = sensorTypes;

        this.Export = new Export();

        var template = new Template(templateName);
        template.SetGroup(new TemplateGroup(groupName));

        this.Export.SetGroup(new ExportGroup(groupName));
        this.Export.SetTemplate(template);
        this.AddItem(this.masterItem);

        this.Gather();
    }

    public void AddItem(IItem item)
    {
        this.Export.AddItem(item);
    }

    public void Gather()
    {
        var computer = new Computer
        {
            IsBatteryEnabled = this.computerHardware.Contains(ComputerHardware.Battery),
            IsControllerEnabled = this.computerHardware.Contains(ComputerHardware.Controller),
            IsCpuEnabled = this.computerHardware.Contains(ComputerHardware.Cpu),
            IsGpuEnabled = this.computerHardware.Contains(ComputerHardware.Gpu),
            IsMemoryEnabled = this.computerHardware.Contains(ComputerHardware.Memory),
            IsMotherboardEnabled = this.computerHardware.Contains(ComputerHardware.Motherboard),
            IsNetworkEnabled = this.computerHardware.Contains(ComputerHardware.Network),
            IsPsuEnabled = this.computerHardware.Contains(ComputerHardware.Psu),
            IsStorageEnabled = this.computerHardware.Contains(ComputerHardware.Storage),
        };

        computer.Open();
        computer.Accept(this);

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

    private void processHardware(IHardware hardware)
    {
        foreach (ISensor sensor in hardware.Sensors)
        {
            if (this.sensorTypes.Contains(sensor.SensorType))
            {
                var item = new Item(hardware, sensor);
                item.SetMasterItem(this.masterItem);
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
        foreach (IHardware subHardware in hardware.SubHardware)
        {
            subHardware.Accept(this);
        }
    }

    public void VisitParameter(IParameter parameter) { }

    public void VisitSensor(ISensor sensor) { }
}
