using Microsoft.Extensions.DependencyInjection;

namespace AddLib;

public interface ILibrary
{
    void ConfigureServices(IServiceCollection services);
}
