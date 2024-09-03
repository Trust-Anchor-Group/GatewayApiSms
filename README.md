Payment Service (Template)
=============================

This repository provides a template solution containing a mock payment service. Developers who want to create a custom payment service for the 
TAG Neuron(R) can use this repository as a template.

Steps to create a custom payment service
--------------------------------------------

1.  Create a new repository based on this template repository
	* Follow naming conventions for repositories, to make the repository easy to find. A Tag Service running on the TAG Neuron(R) typically
	resides in a repository named `NeuronSERVICE`, where `SERVICE` is a short name for the service being implemented.
	* It has been assumed the repository will be cloned to `C:\My Projects\TemplatePaymentService`, see build events below.

2.  Change the solution and project names, as well as the corresponding *manifest file* (see below).
	* Follow naming conventions for libraries, to avoid confusion when navigating code in the Neuron(R): `COMPANY.CATEGORY.SERVICE[.SUBSERVICE]`.

3.  Update the *post build events*, to match the folder and project names you will use.

4.  [Install a Neuron](https://lab.tagroot.io/Documentation/Neuron/InstallBroker.md) on your development machine, that you can use for debugging 
	and testing.
	* Make sure the *IoT Broker* Windows Service is not started (Disabled or Manual), and does not start automatically. You will start it from the
	Visual Studio when debugging. Stop the service if it started automatically after installation.
	* Update the *post build events* so they refer to the tools available in the Neuron installation folder.
	* Alternatively, clone the [IoT Gateway repository](https://github.com/PeterWaher/IoTGateway) and compile it. In the post build events, it is 
	assumed it is cloned to `C:\My Projects\IoT Gateway`. It álso contains the tools referenced from the build script to generate packages.
	
5.  Make the payment service the default *Startup Project*, and edit its *Debug Launch Profile* from *project properties*, so the folders match
	the folders your Neuron was installed at.

6.  Compile and run the template service. Make sure the console version of the Neuron is started. Once started, go to the administation page
	and make sure the *Payment Template* button is available in the *Software* section.

7.  Implement the payment interfaces, as shown in code.
	* Reuse libraries used by the Neuron(R) as much as possible, to simplify distribution and facilitate fixes and updates.
	* Go through all comments in code marked with `TODO`.

8.  Update the Manifest file so it contains all referenced assemblies and content files and folders necessary to install service on a Neuron(R). 
	You do not need to reference assemblies or content files that are part of the Neuron(R) distribution itself.

9.  Create an installable package that can be distributed and installed on TAG Neurons.

10. Compile and test on a local development Neuron(R).

11. Once it works, sign and distribute package on test Neurons, and later production Neurons.

12. Update project documentation for future developers, following documentation style of similar projects, for recognizability and ease of use.

13. Append template documentation with useful hints or information, if needed.

14. Provide a correct license for the repository.

Projects
-----------

The solution contains the following C# projects:

| Project                      | Framework         | Description |
|:-----------------------------|:------------------|:------------|
| `TAG.Payments.Template`      | .NET Standard 2.0 | Payment Mock service that works as a good starting point for developing new payment services for the TAG Neuron(R). |

Nugets
---------

The following external nugets are used. They faciliate common programming tasks, and enables the service to be hosted on the 
[TAG Neuron](https://lab.tagroot.io/Documentation/Index.md) without conflicts. For a list of general nugets available that can
be used, see the [IoT Gateway repository](https://github.com/PeterWaher/IoTGateway).

| Nuget                                                                              | Description |
|:-----------------------------------------------------------------------------------|:------------|
| [Paiwise](https://www.nuget.org/packages/Paiwise)                                  | Contains services for integration of financial services into Neurons. |
| [Waher.Content](https://www.nuget.org/packages/Waher.Content/)                     | Pluggable architecture for accessing, encoding and decoding Internet Content. Include nugets named `Waher.Content.*` to access features of specific Content Types. |
| [Waher.Events](https://www.nuget.org/packages/Waher.Events/)                       | An extensible architecture for event logging in the application. |
| [Waher.IoTGateway](https://www.nuget.org/packages/Waher.IoTGateway/)               | Contains the [IoT Gateway](https://github.com/PeterWaher/IoTGateway) hosting environment. |
| [Waher.Networking](https://www.nuget.org/packages/Waher.Networking/)               | Tools for working with communication, including troubleshooting. Include nugets named `Waher.Networking.*` if you need support for specific communication protocols. |
| [Waher.Runtime.Inventory](https://www.nuget.org/packages/Waher.Runtime.Inventory/) | Maintains an inventory of type definitions in the runtime environment, and permits easy instantiation of suitable classes, and inversion of control (IoC). |
| [Waher.Runtime.Settings](https://www.nuget.org/packages/Waher.Runtime.Settings/)   | Provides easy access to persistent settings. |

Installable Package
----------------------

To create a package, that can be distributed or installed, you begin by creating a *manifest file*. The `TAG.Payments.Template` project 
has a manifest file called `TAG.Payments.Template.manifest`. It defines the assemblies and content files and folders included in the package. 
You then use the `Waher.Utility.Install` and `Waher.Utility.Sign` command-line tools in the [IoT Gateway](https://github.com/PeterWaher/IoTGateway) 
repository, to create a package file and cryptographically sign it for secure distribution across the Neuron network. These tools are also
available in the installation folder of the Neuron(R) distribution.

### Generating Keys

To sign and distribute a package you will need a *public* and *private* key pair. The private key is used for signing the package, and the
public key is used as part of the key required to install a package. Each time you distribute a new package, it must be signed using the same
private key, or the Neuron(R) receiving the new package will discard it. Each new package received is tested if it has been signed using the
same private key. Only if the signature of the new package matches the public key of the installed version, will the new package be accepted
as an update to the installed package.

You will also need an AES key. The package is also encrypted using the symmetric AES cipher. This key is mainly used for obfuscating the
contents of a package.

To generate a new public and private key pair, as well as the AES key, you can execute the following script from a script prompt on the Neuron(R). 
You can find it from the Admin page, in the Lab section. The *installation key* is then the concatenation of `PubKey` and `AesKey`.

```
Key:=Ed448();
printline("PubKey: "+Base64Encode(Key.PublicKey));
printline("PrivKey: "+select /default:EllipticCurve/@d from Xml(Key.Export()));
printline("AesKey: "+Hashes.BinaryToString(Waher.IoTGateway.Gateway.NextBytes(16)));
```

**Security Note**: The Public Key and AES Keys can be distributed together with the package to third parties for installation. They do not represent
a protection by themselves, as they are considered known. The Private Key however, **must not** be distributed or stored in unsecure locations, 
including cloud storage, online repositories, etc. If anyone gets access to the private key, they will be able to create a counterfit package
of the same name.

### Generating package

Once you are ready to create the installable package, you use the `Waher.Utility.Install` tool to create a distributable package, and the
`Waher.Utility.Sign` tool to sign it and create a signature file. The following Command-Line prompt (Windows) provides an example of how this
can be done. Here, it is assumed you are located in the `C:\My Projects` folder on a Windows machine, and use the tools from the compiled
[IoT Gateway](https://github.com/PeterWaher/IoTGateway) repository. You can likewise use the same tools from an installed version of the
TAG Neuron(R) to do this.

```
IoTGateway\Utilities\Waher.Utility.Install\bin\Release\PublishOutput\win-x86\Waher.Utility.Install.exe
	-p TAG.ContentServiceTemplate.package -k [AESKEY]
	-m TemplateContentOnlyPackage\ContentServiceTemplate.manifest

IoTGateway\Utilities\Waher.Utility.Sign\bin\Release\PublishOutput\win-x86\Waher.Utility.Sign.exe 
	-c ed448 
	-priv [PRIVKEY]
	-o TAG.ContentServiceTemplate.signature
	-s TAG.ContentServiceTemplate.package
```

**Note**: The command line example above are only two commands, shown on multiple rows, for readability.

**Note 2**: You need to replace `[AESKEY]` with the value of the `AesKey` generated using the script in the previous section. Likewise, you need
to replace `[PRIVKEY]` with the value of `PrivKey`.

Once the `.package` and `.signature` files are generated, you can upload them to a test Neuron(R). The package will be automatically distributed
to any connected child neurons, recursively. If the signature in the `.signature` file validates using any public key used on a Neuron(R) where
a previous package with the same name has been installed, it will be accepted, otherwise rejected. Depending on update settings on the Neuron(R),
the package will be installed automatically, installed with a delay, or deferred to the operator for manual update or install (the default).

Building, Compiling & Debugging
----------------------------------

The repository assumes you have the [IoT Gateway](https://github.com/PeterWaher/IoTGateway) repository cloned in a folder called
`C:\My Projects\IoT Gateway`, and that this repository is placed in `C:\My Projects\TemplatePaymentService`. You can place the
repositories in different folders, but you need to update the build events accordingly. You can also use an installed Neuron(R)
on your development machine, and use it instead of the IoT Gateway. If you do so, you need to update the build events and debug
profiles to match the installation folder. To run the application, you select the `TAG.Payments.Template` project as your startup 
project. It will execute the console version of the [IoT Gateway](https://github.com/PeterWaher/IoTGateway), and make sure the compiled 
files of the `TemplatePaymentService` solution is run with it.

Gateway.config
-----------------

To simplify development, once the project is cloned, add a `FileFolder` reference to your repository folder in your 
[gateway.config file](https://lab.tagroot.io/Documentation/IoTGateway/GatewayConfig.md). This allows you to test and run your changes to 
Markdown, [back-end script](https://lab.tagroot.io/Script.md) and Javascript immediately, without having to synchronize the folder contents 
with an external host, or recompile or go through the trouble of generating a distributable software package just for testing purposes. 
Changes you make in .NET can be applied in runtime if you the *Hot Reload* permits, otherwise you need to recompile and re-run the application 
again.

Example of how to point a web folder to your project folder:

```
<FileFolders>
  <FileFolder webFolder="/TemplatePayment" folderPath="C:\My Projects\TemplatePaymentService\TAG.Payments.Template\Root\Template"/>
</FileFolders>
```

**Note**: Once the file folder reference is added, you need to restart the Neuron(R) for the change to take effect.

**Note 2**:  Once the Neuron(R) is restarted, the source for the files is taken from the new location. Any changes you make 
in the corresponding `ProgramData` subfolder will have no effect on what you see via the browser.

**Note 3**: This file folder is only necessary on your developer machine, to give you real-time updates as you edit the files in your
development folder. It is not necessary in a production environment, as the files are copied into the correct folders when the package 
is installed.
