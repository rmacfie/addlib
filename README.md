# AddLib

Ever been annoyed that your application's entry point, `Program.cs`, must
know about the whole dependency graph of every project in your solution to
be able to configure the DI container?

Ever been annoyed that you can't mark a class or interface as `internal` because
it makes them difficult to use via Dependency Injection?

AddLib can help you 🎉


## Installation

```sh
dotnet add package AddLib
```

AddLib has a single external reference to the lightweight
`Microsoft.Extensions.DependencyInjection.Abstractions` package, which will
also be installed implicitly.

## How to use

In your library project, add a class that implements `AddLib.ILibrary`:

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace MySolution.Domain;

public class Library : AddLib.ILibrary
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IInternalDomainUtility, InternalDomainUtility>();
        services.AddTransient<IUserDomainService, UserDomainService>();
    }
}
```

In your application entry point, e.g. `Program.cs`, register the Library
by pointing to the type:

```csharp
services.AddLibrary<MySolution.Domain.Library>();
```

Or let AddLib scan assemblies for the `ILibrary` implementations:

```csharp
// Scans the given assembly
services.AddLibrary(typeof(MySolution.Domain.IUserDomainService).Assembly);

// Scans all loaded assemblies matching the pattern
services.AddLibraries("Foo.*");
```

## Making `internal` visible to tests

You might ask "Don't I still need to make all the classes and interfaces
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

or as code:

```csharp
[assembly: InternalsVisibleTo("MySolution.UnitTests")]
```
