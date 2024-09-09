using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using PoC.VirtualManager.Interactions.Slack.Listener.Extensions;
using PoC.VirtualManager.Interactions.Slack.Broadcaster.Extensions;
using PoC.VirtualManager.Interactions.Slack.Plugins.Extensions;
using PoC.VirtualManager.Plugins;
using PoC.VirtualManager.Teams.Plugins.Extensions;
using PoC.VirtualManager.Teams.Client.Extensions;
using PoC.VirtualManager.Brain.FrontalLobe.Personality;
using PoC.VirtualManager.Brain.TemporalLobe;
using PoC.VirtualManager.Interactions.Infrastructure.Extensions;
using Azure.AI.TextAnalytics;
using Azure;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, configuration) =>
    {
        configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((Action<HostBuilderContext, IServiceCollection>)((context, services) =>
    {
        var openAiKey = Environment.GetEnvironmentVariable("VirtualManager_OpenAI_ApiKey") ?? throw new Exception("VirtualManager_OpenAI_ApiKey env variable must be set");
        var personalitySection = context.Configuration.GetSection("Personality");
        var personality = personalitySection.Get<Personality>();

        services.AddSingleton(personality);
        services.AddLogging(l => l.AddConsole().SetMinimumLevel(LogLevel.Trace));

        AddAIServicesWithPugins(services, openAiKey);

        services.AddTeamsStubClient();

        services.AddSingleton(_ =>
        {
            var writer = new StreamWriter(
                path: context.Configuration["FileDirectory"] + $"VirtualManager.txt", //_{DateTime.UtcNow.ToString("yyyyMMddTHHmm")}
                append: true
            );
            writer.AutoFlush = true;

            return writer;
        });

        var slackAccessToken = Environment.GetEnvironmentVariable("VirtualManager_Slack_AccessToken") ?? throw new Exception("VirtualManager_Slack_AccessToken env variable must be set");
        var mongoDbConnectionString = Environment.GetEnvironmentVariable("VirtualManager_MongoDb_ConnectionString") ?? throw new Exception("VirtualManager_MongoDb_ConnectionString env variable must be set");
        services.AddInteractionsRepositories(mongoDbConnectionString);
        services.AddSlackInteractionsListener(slackAccessToken);
        services.AddSlackFeedbackProvider(slackAccessToken);
        //services.AddGoogleMeetsInteractionsListener();

        //services.AddHostedService<PoC.VirtualManager.Brain.FrontalLobe.SlackProcessorBackgroundService>();
        services.AddHostedService<InteractionsEvaluatorBackgroundService>();
        services.AddHostedService<InteractionsSummarizerBackgroundService>();
    }))
    .Build();

await host.RunAsync();

static void AddAIServicesWithPugins(IServiceCollection services, string openAiKey)
{
    var langKey = Environment.GetEnvironmentVariable("VirtualManager_Azure_AI_LanguageKey") ?? throw new Exception("VirtualManager_Azure_AI_LanguageKey env variable must be set");
    var langEndpoint = Environment.GetEnvironmentVariable("VirtualManager_Azure_AI_LanguageEndpoint") ?? throw new Exception("VirtualManager_Azure_AI_LanguageEndpoint env variable must be set");
    Uri endpoint = new(langEndpoint);
    AzureKeyCredential credential = new(langKey);
    services.AddScoped<TextAnalyticsClient>(_ => new TextAnalyticsClient(endpoint, credential));

    #region ~Register Plugins 

    services.AddTeamsKernelPlugins();
    services.AddSlackKernelPlugins();

    services.AddScoped<Kernel>(serviceProvider =>
    {
        var kernelBuilder = Kernel.CreateBuilder();

        kernelBuilder.BindTeamsPlugins(serviceProvider);
        kernelBuilder.BindSlackPlugins(serviceProvider);

        return kernelBuilder.Build();
    });

    #endregion

    services.AddOpenAIChatCompletion("gpt-4o-mini", openAiKey);
}