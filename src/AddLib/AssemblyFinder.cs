using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AddLib;

internal static class AssemblyFinder
{
    public static IEnumerable<Assembly> FindAssembliesByName(string wildcardPattern)
    {
        var wildcardMatcher = new WildcardMatcher(wildcardPattern);

        var matchingAssemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(assembly => wildcardMatcher.IsMatch(assembly.GetName().Name ?? ""))
            .ToArray();

        foreach (var assembly in matchingAssemblies)
        {
            AppDomain.CurrentDomain.Load(assembly.FullName);
        }

        return matchingAssemblies;
    }
}
