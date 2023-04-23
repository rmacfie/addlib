using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AddLib.Example.Lib;

public class Library : ILibrary
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ICalculator, Calculator>();
        services.AddTransient<IAdditionHelper, AdditionHelper>();
    }
}
