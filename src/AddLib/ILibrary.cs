using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AddLib;

/// <summary>
///     Specifies the contract for dependency configuration in a class library.
/// </summary>
public interface ILibrary
{
    /// <summary>
    ///     Specifies the method that is used to configure services and dependencies.
    /// </summary>
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);
}
