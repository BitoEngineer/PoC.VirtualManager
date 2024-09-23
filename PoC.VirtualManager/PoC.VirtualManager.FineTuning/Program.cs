

//TODO build CompanyEmployees.json invoking Teams API

//TODO keep track of the scripts that already ran 


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PoC.VirtualManager.FineTuning;
using PoC.VirtualManager.FineTuning.Infrastructure;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, configuration) =>
    {
        var appsettingsName = "appsettings.json";
        configuration.AddJsonFile(appsettingsName, optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IDataSetControlRepository>(_ => new DataSetControlRepository(
            mongoDbConnectionString: "TODO",
            databaseName: "TODO"
        ));

        services.AddHostedService<FinetuningBackgroundService>();
    })
    .Build();

await host.RunAsync();
