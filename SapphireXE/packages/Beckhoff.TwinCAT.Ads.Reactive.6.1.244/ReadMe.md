## Description

The package **'Beckhoff.TwinCAT.Ads.Reactive'** implements reactive extensions for the TwinCAT.Ads.AdsClient class included in the
**'Beckhoff.TwinCAT.Ads'** package.

Reactive Extensions (Rx) is a .NET library for composing asynchronous and event-based programs using observable sequences
and LINQ-style query operators. Using Rx, developers represent asynchronous data streams with Observables, query asynchronous
data streams using LINQ operators, and parameterize the concurrency in the asynchronous data streams using Schedulers.

Simply put, Rx = Observables + LINQ + Schedulers.

In ADS terms, not only the reading and writing data or symbol values can be put into reactive data streams, also ADS Notifications or polling of values are a perfect
fit for reactive code.
With the help of this package not only data binding to reactive frameworks (e.g. reactive UI) is simplified but also enhanced data manipulation via synchronous and asynchronous
observers. 
Multithreaded and parallelized code paths that support multiple CPU cores can be written very easily without the burden of deadlock and synchronization issues.

## Requirements

- **.NET 7.0**, **.NET 6.0** or **.NET Standard 2.0** (e.g. >= **.NET Framework 4.61**) compatible SDK
- A **TwinCAT 3.1.4024** Build (XAE, XAR or ADS Setup) or alternatively the Beckhoff.TwinCAT.AdsRouterConsole Application.
- Installed Nuget package manager (for systems without Visual Studio installation).

## Installation

The 'Beckhoff.TwinCAT.Ads.Reactive' depends on the 'Beckhoff.TwinCAT.Ads' packages and inherits the same requirements (>= TwinCAT 4024.10).

## Version Support lifecycle

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

The following code instantiates an AdsClient object, connects to the local PLC System Port 851 and subscribes fir the first 20 change notifications
of the Task cycle count and writes the value to the Console.

```csharp
// To Test the Observer run a project on the local PLC System (Port 851)
using (AdsClient client = new AdsClient())
{
    // Connect to target
    client.Connect(new AmsAddress(AmsNetId.Local, 851));

    // Reactive Notification Handler
    var valueObserver = Observer.Create<ushort>(val =>
    {
        Console.WriteLine(string.Format("Value: {0}", val.ToString()));
    }
    );

    // Turning ADS Notifications into sequences of Value Objects (Taking 20 Values)
    // and subscribe to them.
    IDisposable subscription = client.WhenNotification<ushort>("TwinCAT_SystemInfoVarList._TaskInfo.CycleCount", NotificationSettings.Default).Take(20).Subscribe(valueObserver);

    Console.ReadKey(); // Wait for Key press
    subscription.Dispose(); // Dispose the Subscription
}
```

## Further documentation

The actual version of the documentation is available in the Beckhoff Infosys.
[Beckhoff Information System](https://infosys.beckhoff.com/index.php?content=../content/1033/tc3_ads.net/index.html&id=207622008965200265)

## Sample Code

[Beckhoff GitHub BaseSamples](https://github.com/Beckhoff/TF6000_ADS_DOTNET_V5_Samples/tree/main/Sources/BaseSamples)
[Beckhoff GitHub ClientSamples](https://github.com/Beckhoff/TF6000_ADS_DOTNET_V5_Samples/tree/main/Sources/ClientSamples)