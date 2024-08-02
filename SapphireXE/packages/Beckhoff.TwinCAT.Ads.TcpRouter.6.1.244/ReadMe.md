## Description

The package **'Beckhoff.TwinCAT.Ads.TcpRouter'** implements a lean TCP ADS Router class to use on systems where no standard TwinCAT router is established or available.

It is running in UserMode only (no realtime characteristics) and contains no further functionality than distributing the ADS Frames (e.g. no Port 10000, no ADS Secure). It is just used to route ADS frames locally between AdsServers 
and to/from remote ADS devices.

Implemented in asynchronous .NET Code it can be run in your own services/daemon, as standalone console application and also in your customized application.

## Requirements

- **.NET 7.0**, **.NET 6.0** or **.NET Standard 2.0** (e.g. >= **.NET Framework 4.61**) compatible SDK
- No other System allocating the same port (e.g. a regular TwinCAT installation).
- Installed Nuget package manager or Dotnet CLI.

## Installation

Along with the deployment of the application where the TcpRouter is implemented, a valid Router / ADS configuration must be placed to specify
the Local Net ID, the name and the default port of the Router system.

The preferred way to configure the system is with standard Configuration providers, which are part of the
.NET Core / ASP .NET Core infrastructure.

See further information:
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1

This enables common options for application configuration that can be used 'out-of-the-box':

- Via the file appsettings.json
- With the StaticRoutesConfigurationProvider (StaticRoutes.xml)
- Using Environment Variables.
- Command line arguments
- etc.

The configuration has to be loaded during application startup and is placed into the **'TwinCAT.Ads.TcpRouter.AmsTcpIpRouter'** class via constructor dependency injection and
must contain the following information:

- The name of the local System (usually the Computer or Hostname)
- The Local AmsNetId of the local system as Unique Address in the network
- Optionally the used TcpPort (48898 or 0xBF02 by default)
- The static routes in the 'RemoteConnections' list.
- Logging configuration.

Actually the configuration is not reloaded during the runtime of the **'TwinCAT.Ads.TcpRouter.AmsTcpIpRouter'** class.
Please be aware that the "Backroute" from the Remote system linking to the local system (via AmsNetId) is necessary also to get functional routes.

Example for a valid 'appSettings.json' file (please change the Addresses for your network/systems.)

```json
{
  "AmsRouter": {
    "Name": "MyLocalSystem",
    "NetId": "192.168.1.20.1.1",
    "TcpPort": 48898,
    "RemoteConnections": [
      {
        "Name": "RemoteSystem1",
        "Address": "RemoteSystem1",
        "NetId": "192.168.1.21.1.1",
        "Type": "TCP_IP"
      },
      {
        "Name": "RemoteSystem2",
        "Address": "192.168.1.22",
        "NetId": "192.168.1.22.1.1",
        "Type": "TCP_IP"
      },
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "System": "Information",
      "Microsoft": "Information"
    },
    "Console": {
      "IncludeScopes": true
    }
  }
}
```

Alternatively a "StaticRoutes.Xml" Xml File can configure the system equally. Don't forget to add the **'StaticRoutesXmlConfigurationProvider'** to the Host configuration
during startup (see FirstSteps below).

An example of the local "StaticRoutes.xml" is given here:

```xml
<?xml version="1.0" encoding="utf-8"?>
<TcConfig xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="C:\TwinCAT3\Config\TcConfig.xsd">
  <Local>
      <Name>MyLocalSystem</Name>
      <NetId>192.168.1.20.1.1</NetId> <!-- Local NetId -->
      <TcpPort>48898</TcpPort> <!-- Default TcpPort -->
  </Local>
  <RemoteConnections>
    <Route>
      <Name>RemoteSystem1</Name>
      <Address>RemoteSytem</Address> <!-- HostName -->
      <!--<Address>192.168.1.21</Address>  --> <!--IPAddress -->
      <NetId>192.168.1.21.1.1</NetId>
      <Type>TCP_IP</Type>
    </Route>
    <Route>
      <Name>RemoteSystem2</Name>
      <Address>192.168.1.22</Address> <!-- IPAddress -->
      <!--<Address>RemoteSystem2</Address>  --> <!--HostName -->
      <NetId>192.168.1.21.1.1</NetId>
      <Type>TCP_IP</Type>
    </Route>
  </RemoteConnections>
</TcConfig>
```

Alternatively, the configuration can also be set via Environment variables.

```Powershell
PS> $env:AmsRouter:Name = 'MyLocalSystem'
PS> $env:AmsRouter:NetId = '192.168.1.20.1.1'
PS> $env:AmsRouter:TcpPort = 48898
PS> $env:AmsRouter:RemoteConnections:0:Name = 'RemoteSystem1'
PS> $env:AmsRouter:RemoteConnections:0:Address = 'RemoteSystem1'
PS> $env:AmsRouter:RemoteConnections:0:NetId = '192.168.1.21.1.1'
PS> $env:AmsRouter:RemoteConnections:1:Name = 'RemoteSystem2'
PS> $env:AmsRouter:RemoteConnections:1:Address = '192.168.1.22'
PS> $env:AmsRouter:RemoteConnections:1:NetId = '192.168.1.22.1.1'
PS> $env:AmsRouter:Logging:LogLevel:Default = 'Information'
```

```Powershell
PS> dir env: | where Name -like AmsRouter* | format-table -AutoSize

Name                                  Value
----                                  -----
AmsRouter:Name                        MyLocalSystem
AmsRouter:NetId                       192.168.1.20.1.1
AmsRouter:TcpPort                     48898
AmsRouter:RemoteConnections:0:Name    RemoteSystem1
AmsRouter:RemoteConnections:0:Address RemoteSystem1
AmsRouter:RemoteConnections:0:NetId   192.168.1.21.1.1
AmsRouter:RemoteConnections:1:Name    RemoteSystem2
AmsRouter:RemoteConnections:1:Address 192.168.1.22
AmsRouter:RemoteConnections:1:NetId   192.168.1.22.1.1
AmsRouter:Logging:LogLevel:Default    Information
```

### Configuration Parameters

| Name | Description |
| ---- | ----------- |
| Name | Name of the local System/Device |
| NetId | The AmsNetId of the local System/device |
| TcpPort | The TCP port used for external communication (communication to the routes/RemoteConnections)|
| LoopbackIP | This is the IPAddress, that is used by the TcpRouter for its Loopback Connections (in combination with the LoopbackPort. By default this is set to IPAddress.Loopback (127.0.0.1) and is only accessible from the local machine. If AdsClient/AdsServers should run seperated from the Router System, this LoopbackIP must be set to valid local IPAddress. Furthermore valid external addresses (where the AdsClients/AdsServer lives) must be specified via LoopbackExternalIPs or LoopbackExternalSubnet. Only those connections will be accepted|
| LoopbackPort | Sets the TCP Port that is used for the loopback. The LoopbackPort defines the Loopback **TcpEndpoints** in combination with the **LoopbackIP** | 
| LoopbackExternalIPs | The Loopback externals are IPAddresses, that are allowed to use the Loopback connection. Use this IP list or specify alternatively the **LoopbackExternalSubnet**|
| LoopbackExternalSubnet | Sets the loopback externals subnet. This is an alternative approach to set the allowed **'LoopbackIPs'** for loopback communication. In docker/virtual enviroments often a whole subnet will be spanned|
| RemoteConnections | Sets the list of remote Routes/Connections. This is the list of external devices which can be reached via the route.|

### Version Support lifecycle

| Package | Description | .NET Framework | TwinCAT | Active Support |
|---------|-------------|----------------|---------|-----------------
6.1 | Package basing on .NET 7.0/6.0 | net7.0, net6.0, netstandard2.0 | >= 3.1.4024.10 [^1] | X |
6.0 | Package basing on .NET 6.0 | net6.0, netcoreapp3.1, netstandard2.0, net461 | >= 3.1.4024.10 [^1] | X |
5.x | Package basing on .NET 5.0[^3] | net5.0, netcoreapp3.1, netstandard2.0, net461 | >= 3.1.4024.10 [^1] | |
4.x | Package basing on .NET Framework 4.0 | net4 | All | X |

[^1]: Requirement on the Host system. No version limitation in remote system communication.

[^2]: Microsoft support for .NET5 ends with May 8, 2022. Therefore it is recommended to update **Beckhoff.TwinCAT** packages from Version 5 to Version 6.

[Migrate from ASP.NET Core 5.0 to 6.0](https://docs.microsoft.com/en-us/aspnet/core/migration/50-to-60?view=aspnetcore-6.0&tabs=visual-studio)

[migrating to the latest .NET](https://docs.microsoft.com/en-us/dotnet/architecture/modernize-desktop/example-migration)
[Microsoft .NET support lifecycle](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core)

## First Steps

Example of starting the TcpIpRouter from a simple Console application with logging.

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TwinCAT.Ads.AdsRouterService
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<RouterWorker>();
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // Uncomment to overwrite configuration
                    //config.Sources.Clear(); // Clear all default config sources 
                    //config.AddEnvironmentVariables("AmsRouter"); // Use Environment variables
                    //config.AddCommandLine(args); // Use Command Line
                    //config.AddJsonFile("appSettings.json"); // Use Appsettings
                    //config.AddStaticRoutesXmlConfiguration(); // Overriding settings with StaticRoutes.Xml 
                })
                .ConfigureLogging(logging =>
                {
                    // Uncomment to overwrite logging
                    // Microsoft.Extensions.Logging.Console Nuget package
                    // Namespace Microsoft.Extensions.Logging;
                    //logging.ClearProviders();
                    //logging.AddConsole();
                })
            ;
    }
}
```

```csharp
public class RouterWorker : BackgroundService
{
    private readonly ILogger<RouterWorker> _logger;

    public RouterWorker(ILogger<RouterWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancel)
    {
        AmsTcpIpRouter router = new AmsTcpIpRouter(_logger);

        //Use this overload to instantiate a Router without support of StaticRoutes.xml and parametrize by code
        //AmsTcpIpRouter router = new AmsTcpIpRouter(new AmsNetId("1.2.3.4.5.6"), AmsTcpIpRouter.DEFAULT_TCP_PORT, _logger);
        //router.AddRoute(...);

        await router.StartAsync(cancel); // Start the router
    }
}
```
## Further documentation

The actual version of the documentation is available in the Beckhoff Infosys.
[Beckhoff Information System](https://infosys.beckhoff.com/index.php?content=../content/1033/tc3_ads.net/index.html&id=207622008965200265)

## Sample Code

[Beckhoff GitHub RouterSamples](https://github.com/Beckhoff/TF6000_ADS_DOTNET_V5_Samples/tree/main/Sources/RouterSamples)
