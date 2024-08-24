using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using PoC.VirtualManager;
using PoC.VirtualManager.Interactions.Slack.Listener.Extensions;
using PoC.VirtualManager.Interactions.GoogleMeets.Listener.Extensions;
using PoC.VirtualManager.Personality;
using PoC.VirtualManager.Plugins;
using PoC.VirtualManager.Teams.Client.Extensions;
using System.Threading.Channels;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, configuration) =>
    {
        configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var openAiKey = Environment.GetEnvironmentVariable("VirtualManager_OpenAI_ApiKey") ?? throw new Exception("AI:OpenAI:ApiKey env variable must be set");
        var personalitySection = context.Configuration.GetSection("Personality");
        var personality = personalitySection.Get<Personality>();

        services.AddSingleton(personality);

        services.AddLogging(l => l.AddConsole().SetMinimumLevel(LogLevel.Trace));
        services.AddOpenAIChatCompletion("gpt-4o-mini", openAiKey);
        services.AddTeamsStubClient();
        services.AddKernel();
        services.AddTeamsKernelPlugins();

        services.AddKeyedSingleton("interactionsChannel", Channel.CreateUnbounded<string>());
        services.AddSingleton(_=> {
            var writer = new StreamWriter(
                path: context.Configuration["FileDirectory"] + $"VirtualManager.txt", //_{DateTime.UtcNow.ToString("yyyyMMddTHHmm")}
                append: true
            );
            writer.AutoFlush = true;

            return writer;
            });

        services.AddSlackInteractionsListener();
        services.AddGoogleMeetsInteractionsListener();
        services.AddHostedService<Brain>();
    })
    .Build();

await host.RunAsync();
