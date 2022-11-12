using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AddLib;

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class AddLibServiceCollectionExtensions
{
    public static IServiceCollection AddLibraries(this IServiceCollection services, string assemblyNamePattern)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (string.IsNullOrEmpty(assemblyNamePattern))
            throw new ArgumentException("Value cannot be null or empty.", nameof(assemblyNamePattern));

        var matchingAssemblies = AssemblyFinder.FindAssembliesByName(assemblyNamePattern);
        return services.AddLibraries(matchingAssemblies);
    }

    public static IServiceCollection AddLibrary(this IServiceCollection services, Assembly assembly)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (assembly == null)
            throw new ArgumentNullException(nameof(assembly));

        return services.AddLibrary(assembly, true);
    }

    public static IServiceCollection AddLibrary<TLibrary>(this IServiceCollection services)
        where TLibrary : ILibrary, new()
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var instance = new TLibrary();
        return services.AddLibrary(instance);
    }

    public static IServiceCollection AddLibrary(this IServiceCollection services, Type libraryType)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (libraryType == null)
            throw new ArgumentNullException(nameof(libraryType));

        if (Activator.CreateInstance(libraryType) is not ILibrary instance)
            throw new InvalidOperationException(
                $"Failed to create an instance of '{libraryType.FullName}'"
            );

        return services.AddLibrary(instance);
    }

    public static IServiceCollection AddLibrary(this IServiceCollection services, ILibrary instance)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (instance == null)
            throw new ArgumentNullException(nameof(instance));

        instance.ConfigureServices(services);
        return services;
    }

    private static IServiceCollection AddLibraries(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            services.AddLibrary(assembly, false);
        }

        return services;
    }

    private static IServiceCollection AddLibrary(
        this IServiceCollection services, Assembly assembly, bool throwIfNotFound)
    {
        var candidates = assembly.GetExportedTypes()
            .Where(t => !t.IsAbstract && typeof(ILibrary).IsAssignableFrom(t))
            .ToArray();

        if (candidates.Length == 0 && throwIfNotFound)
            throw new ArgumentException(
                $"Assembly {assembly.GetName().Name} does not have a public, non-abstract {nameof(ILibrary)} implementation"
            );

        if (candidates.Length > 1)
            throw new ArgumentException(
                $"Assembly {assembly.GetName().Name} has multiple public, non-abstract {nameof(ILibrary)} implementations"
            );

        return candidates.Length == 0 ? services : services.AddLibrary(candidates[0]);
    }
}
