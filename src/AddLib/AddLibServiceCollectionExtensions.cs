using System;
using System.Linq;
using System.Reflection;
using AddLib;
using Microsoft.Extensions.Configuration;

// ReSharper disable CheckNamespace
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace Microsoft.Extensions.DependencyInjection;

public static class AddLibServiceCollectionExtensions
{
    /// <summary>
    ///     Scans an <see cref="Assembly" /> for an <see cref="ILibrary" /> implementation,
    ///     and applies its registration to the <see cref="IServiceCollection" />.
    /// </summary>
    public static IServiceCollection AddLibrary(
        this IServiceCollection services,
        Assembly assembly,
        IConfiguration configuration
    )
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (assembly == null)
            throw new ArgumentNullException(nameof(assembly));

        return services.AddLibrary(assembly, configuration, true);
    }

    /// <summary>
    ///     Applies the service registration of an <see cref="ILibrary" />
    ///     implementation to the <see cref="IServiceCollection" />.
    /// </summary>
    public static IServiceCollection AddLibrary<TLibrary>(
        this IServiceCollection services,
        IConfiguration configuration
    )
        where TLibrary : ILibrary
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        return services.AddLibrary(typeof(TLibrary), configuration);
    }

    /// <summary>
    ///     Applies the service registration of an <see cref="ILibrary" />
    ///     implementation to the <see cref="IServiceCollection" />.
    /// </summary>
    public static IServiceCollection AddLibrary(
        this IServiceCollection services,
        Type libraryType,
        IConfiguration configuration
    )
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (libraryType == null)
            throw new ArgumentNullException(nameof(libraryType));

        if (!typeof(ILibrary).IsAssignableFrom(libraryType))
            throw new ArgumentException(
                "The type must be an implementation of AddLib.ILibrary.",
                nameof(libraryType)
            );

        if (Activator.CreateInstance(libraryType) is not ILibrary instance)
            throw new InvalidOperationException(
                $"Failed to create an instance of '{libraryType.FullName}'"
            );

        return services.AddLibrary(instance, configuration);
    }

    /// <summary>
    ///     Applies the service registration of an <see cref="ILibrary" />
    ///     implementation to the <see cref="IServiceCollection" />.
    /// </summary>
    public static IServiceCollection AddLibrary(
        this IServiceCollection services,
        ILibrary instance,
        IConfiguration configuration
    )
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (instance == null)
            throw new ArgumentNullException(nameof(instance));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        instance.ConfigureServices(services, configuration);
        return services;
    }

    private static IServiceCollection AddLibrary(
        this IServiceCollection services,
        Assembly assembly,
        IConfiguration configuration,
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

        return candidates.Length == 0
            ? services
            : services.AddLibrary(candidates[0], configuration);
    }
}
