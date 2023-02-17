# zabbix-agent-lhm

`zabbix-agent-lhm` is a C# program that integrates [Zabbix agent](https://www.zabbix.com/documentation/current/en/manual/concepts/agent) with [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor). It uses the `LibreHardwareMonitorLib` library to provide sensor data in a template format that can be imported to Zabbix.

## Installation

The release is a single executable file. Extract it anywhere on your system.

Install the Zabbix agent if not already installed, and add a user parameter to its configuration:

    UserParameter=lhm.gather,zabbix-agent-lhm.exe gather

The `lhm` part in the key `lhm.gather` can be adjusted if needed.

Restart the Zabbix agent service to load the new configuration.

## Configuration

Generate a Zabbix template for your computer configuration. You will likely need to run the command in an administrative command prompt to read all sensors.

    zabbix-agent-lhm.exe template --output "C:\zabbix-agent-lhm.yml"

See the usage section below for available command line options.

Import the YAML template to Zabbix, and assign the template to a host. Note that templates are host specific unless you have identical hardware.

## Usage

The `gather` subcommand gathers sensor data, and the `template` subcommand generates a Zabbix template for the computer configuration.

### gather

    zabbix-agent-lhm.exe gather --help
    Description:
      Gather sensor data

    Usage:
      zabbix-agent-lhm gather [options]

    Options:
      -p, --prefix <prefix>                  Prefix to use for Zabbix keys [default: lhm]
      -s, --sensor-types <sensor-types>      Comma separated list of sensor types to gather [default: fan,power,temperature]
      -t, --hardware-types <hardware-types>  Comma separated list of hardware types to gather [default: cpu,motherboard,storage]
      -?, -h, --help                         Show help and usage information

### template

    zabbix-agent-lhm.exe template --help
    Description:
      Write Zabbix template

    Usage:
      zabbix-agent-lhm template [options]

    Options:
      -o, --output <output>                  Save Zabbix template to this file instead of stdout
      -p, --prefix <prefix>                  Prefix to use for Zabbix keys [default: lhm]
      -s, --sensor-types <sensor-types>      Comma separated list of sensor types to gather [default: fan,power,temperature]
      -t, --hardware-types <hardware-types>  Comma separated list of hardware types to gather [default: cpu,motherboard,storage]
      -g, --template-group <template-group>  Name of the Zabbix template group [default: Templates/LibreHardwareMonitor]
      -n, --template-name <template-name>    Name of the Zabbix template [default: Template App LibreHardwareMonitor]
      -?, -h, --help                         Show help and usage information

# Licenses

This project is licensed under the [MIT license](LICENSE).

[LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor/) is licensed under the Mozilla Public License Version 2.0.

[YamlDotNet](https://github.com/aaubry/YamlDotNet) is licensed under its respective license.

[command-line-api](https://github.com/dotnet/command-line-api) is licensed under the MIT license.
