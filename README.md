# AddLib ![Nuget](https://img.shields.io/nuget/v/AddLib)

Ever been annoyed that your application's entry point, `Program.cs`, must
know about the whole dependency graph of every project in your solution to
be able to configure the DI container?

Ever been annoyed that you can't mark a class or interface as `internal` because
it makes them difficult to use via Dependency Injection?

AddLib can help you 🎉

AddLib provides a simple convention for encapsulating the internal types and
functionality of class libraries.


## Installation

```sh
dotnet add package AddLib
```

AddLib has a two external references to the lightweight
`Microsoft.Extensions.DependencyInjection.Abstractions` and
`Microsoft.Extensions.Configuration.Abstractions` packages, which will
also be installed implicitly.


## How to use

In your class library project, add a class that implements `AddLib.ILibrary`:

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MySolution.Domain;

public class Library : AddLib.ILibrary
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddTransient<IDatabaseContext>(x => new DatabaseContext(connectionString));
        services.AddTransient<IInternalDomainUtility, InternalDomainUtility>();
        services.AddTransient<IUserDomainService, UserDomainService>();
    }
}
```

In your application entry point, e.g. `Program.cs`, register the Library
by pointing to the type:

```csharp
services.AddLibrary<MySolution.Domain.Library>(configuration);
```

Or let AddLib scan assemblies for the `ILibrary` implementations:

```csharp
// Scans the given assembly
services.AddLibrary(typeof(MySolution.Domain.IUserDomainService).Assembly, configuration);

// Scans all loaded assemblies matching a pattern
services.AddLibraries("MySolution.*", configuration);
```


## Making `internal` visible to tests

You might ask: "Don't I still need to make all the classes and interfaces
`public` so I can reference them in test projects?"

Not necessarily. You can use the `InternalsVisibleTo` attribute.

Either in your `Project.csproj` or `Directory.Build.props`:

```xml
<ItemGroup>
  <AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleToAttribute">
    <_Parameter1>MySolution.UnitTests</_Parameter1>
  </AssemblyAttribute>
</ItemGroup>
```

Or as code, placed anywhere in your library project:

```csharp
[assembly: InternalsVisibleTo("MySolution.UnitTests")]
```
