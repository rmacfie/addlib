using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AddLib;

// ReSharper disable CheckNamespace
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace Microsoft.Extensions.DependencyInjection;

public static class AddLibServiceCollectionExtensions
{
    /// <summary>
    ///     Finds assemblies whose names matches the <paramref name="assemblyNamePattern" />,
    ///     scans each assembly for an <see cref="ILibrary" /> implementation,
    ///     and applies its configuration to the <see cref="IServiceCollection" />.
    ///     The assembly name pattern uses <c>*</c> to match one or many characters and
    ///     <c>?</c> to match a single character.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="assemblyNamePattern">
    ///     The assembly name pattern, e.g. <c>"Acme.Domain.*"</c>.
    /// </param>
    public static IServiceCollection AddLibraries(
        this IServiceCollection services,
        string assemblyNamePattern
    )
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (string.IsNullOrEmpty(assemblyNamePattern))
            throw new ArgumentException(
                "Value cannot be null or empty.",
                nameof(assemblyNamePattern)
            );

        var matchingAssemblies = AssemblyFinder.FindAssembliesByName(assemblyNamePattern);
        return services.AddLibraries(matchingAssemblies);
    }

    /// <summary>
    ///     Scans an <see cref="Assembly" /> for an <see cref="ILibrary" /> implementation,
    ///     and applies its configuration to the <see cref="IServiceCollection" />.
    /// </summary>
    public static IServiceCollection AddLibrary(this IServiceCollection services, Assembly assembly)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (assembly == null)
            throw new ArgumentNullException(nameof(assembly));

        return services.AddLibrary(assembly, true);
    }

    /// <summary>
    ///     Applies the service configuration of an <see cref="ILibrary" />
    ///     implementation to the <see cref="IServiceCollection" />.
    /// </summary>
    public static IServiceCollection AddLibrary<TLibrary>(this IServiceCollection services)
        where TLibrary : ILibrary
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        return services.AddLibrary(typeof(TLibrary));
    }

    /// <summary>
    ///     Applies the service configuration of an <see cref="ILibrary" />
    ///     implementation to the <see cref="IServiceCollection" />.
    /// </summary>
    public static IServiceCollection AddLibrary(this IServiceCollection services, Type libraryType)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (libraryType == null)
            throw new ArgumentNullException(nameof(libraryType));

        if (!libraryType.IsSubclassOf(typeof(ILibrary)))
            throw new ArgumentException(
                "The type must be an implementation of AddLib.ILibrary.",
                nameof(libraryType)
            );

        if (Activator.CreateInstance(libraryType) is not ILibrary instance)
            throw new InvalidOperationException(
                $"Failed to create an instance of '{libraryType.FullName}'"
            );

        return services.AddLibrary(instance);
    }

    /// <summary>
    ///     Applies the service configuration of an <see cref="ILibrary" />
    ///     implementation to the <see cref="IServiceCollection" />.
    /// </summary>
    public static IServiceCollection AddLibrary(this IServiceCollection services, ILibrary instance)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (instance == null)
            throw new ArgumentNullException(nameof(instance));

        instance.ConfigureServices(services);
        return services;
    }

    private static IServiceCollection AddLibraries(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies
    )
    {
        foreach (var assembly in assemblies)
        {
            services.AddLibrary(assembly, false);
        }

        return services;
    }

    private static IServiceCollection AddLibrary(
        this IServiceCollection services,
        Assembly assembly,
        bool throwIfNotFound
    )
    {
        var candidates = assembly
            .GetExportedTypes()
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
