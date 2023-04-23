using AddLib.Example.App;
using AddLib.Example.Lib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var config = new ConfigurationManager();

var services = new ServiceCollection()
    .AddLibrary<Library>(config)
    // .AddLibrary(typeof(Library), config)
    // .AddLibrary(new Library(), config)
    // .AddLibrary(typeof(ICalculator).Assembly, config)
    .AddTransient<IMessageFactory, MessageFactory>()
    .BuildServiceProvider();

var messageFactory = services.GetRequiredService<IMessageFactory>();

Console.WriteLine(messageFactory.GetMessage());
