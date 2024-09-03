Sending SMS using GatewayAPI.com 
===================================

This repository contains a communication library with a script extension for sending SMS using [GatewayAPI.com](GatewayAPI.com).

Projects
-----------

The solution contains the following C# projects:

| Project                          | Framework         | Description |
|:---------------------------------|:------------------|:------------|
| `TAG.Networking.GatewayApi`      | .NET Standard 2.0 | Communication library that implements the GatewayAPI.com REST API. |
| `TAG.Networking.GatewayApi.Test` | .NET 8            | Unit tests for the `TAG.Networking.GatewayApi` library.            |
| `TAG.Service.GatewayApi`         | .NET Standard 2.0 | Service module that integrates GatewayAPI.com into a Neuron(R). Provides a configuration page and script extension. |

Nugets
---------

The following external nugets are used. They faciliate common programming tasks, and enables the service to be hosted on the 
[TAG Neuron](https://lab.tagroot.io/Documentation/Index.md) without conflicts. For a list of general nugets available that can
be used, see the [IoT Gateway repository](https://github.com/PeterWaher/IoTGateway).

| Nuget                                                                              | Description |
|:-----------------------------------------------------------------------------------|:------------|
| [Waher.Content](https://www.nuget.org/packages/Waher.Content/)                     | Pluggable architecture for accessing, encoding and decoding Internet Content. Include nugets named `Waher.Content.*` to access features of specific Content Types. |
| [Waher.Events](https://www.nuget.org/packages/Waher.Events/)                       | An extensible architecture for event logging in the application. |
| [Waher.Events.Console](https://www.nuget.org/packages/Waher.Events.Console/)       | A library that outputs logged events to the console output. |
| [Waher.IoTGateway](https://www.nuget.org/packages/Waher.IoTGateway/)               | Contains the [IoT Gateway](https://github.com/PeterWaher/IoTGateway) hosting environment. |
| [Waher.Networking](https://www.nuget.org/packages/Waher.Networking/)               | Tools for working with communication, including troubleshooting. Include nugets named `Waher.Networking.*` if you need support for specific communication protocols. |
| [Waher.Runtime.Inventory](https://www.nuget.org/packages/Waher.Runtime.Inventory/) | Maintains an inventory of type definitions in the runtime environment, and permits easy instantiation of suitable classes, and inversion of control (IoC). |
| [Waher.Runtime.Settings](https://www.nuget.org/packages/Waher.Runtime.Settings/)   | Provides easy access to persistent settings. |
| [Waher.Security](https://www.nuget.org/packages/Waher.Security/)                   | Basic encryption-related tools. |

Installable Package
----------------------

The service has been made into a package that can be downloaded and installed on any [TAG Neuron](https://lab.tagroot.io/Documentation/Index.md). 
If your Neuron(R) is connected to this network, you can install the package using the following information:

| Package information                                                                                                              ||
|:-----------------|:---------------------------------------------------------------------------------------------------------------|
| Package          | `TAG.Service.GatewayApi.package`                                                                               |
| Installation key | `qHh3N01htgWDSci38o+U1c4O76prAaZ54FjGbweFLUcHmWNZn1WtORx+87Z+xZFydk886sxDRBAAb900fba31cac92078bed72e8d89fda7e` |
| More Information | TBD                                                                                                            |

## Development

The service runs on any [IoT Gateway Host](https://github.com/PeterWaher/IoTGateway). This includes the
[TAG Neuron(R)](https://lab.tagroot.io/Documentation/Index.md), The IoT Gateway itself, or [Lil'Sis'](https://lils.is/), 
which you can run on your local machine. To simplify development, once the project is cloned, add a `FileFolder` reference
to your repository folder in your [gateway.config file](https://lab.tagroot.io/Documentation/IoTGateway/GatewayConfig.md). 
This allows you to test and run your changes immediately, without having to synchronize the folder contents with an external 
host, or go through the trouble of generating a distributable software package just for testing purposes.

Example:

```
<FileFolders>
  <FileFolder webFolder="/GatewayApi" folderPath="C:\My Projects\GatewayApiSms\TAG.Service.GatewayApi\Root\GatewayApi"/>
</FileFolders>
```

**Note**: Once file folder reference is added, you need to restart the IoT Gateway service for the change to take effect.

## Solution File

A Visual Studio solution file, with references to the files and folders of this repository, is available: `GatewayApiSms.sln`.

## Main Page

The main page of the community service is `/GatewayApi/Settings.md`.

Building, Compiling & Debugging
----------------------------------

The repository assumes you have the [IoT Gateway](https://github.com/PeterWaher/IoTGateway) repository cloned in a folder called
`C:\My Projects\IoT Gateway`, and that this repository is placed in `C:\My Projects\GatewayApiSms`. You can place the
repositories in different folders, but you need to update the build events accordingly. You can also use an installed Neuron(R)
on your development machine, and use it instead of the IoT Gateway. If you do so, you need to update the build events and debug
profiles to match the installation folder. To run the application, you select the `TAG.Service.GatewayApi` project as your startup 
project. It will execute the console version of the [IoT Gateway](https://github.com/PeterWaher/IoTGateway), and make sure the compiled 
files of the `GatewayApiSms` solution is run with it.

