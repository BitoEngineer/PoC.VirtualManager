using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using PoC.VirtualManager.Interactions.Slack.Listener.Extensions;
using PoC.VirtualManager.Interactions.Slack.Broadcaster.Extensions;
using PoC.VirtualManager.Interactions.Slack.Plugins.Extensions;
using PoC.VirtualManager.Interactions.Slack.Client.Extensions;
using PoC.VirtualManager.Plugins;
using PoC.VirtualManager.Teams.Plugins.Extensions;
using PoC.VirtualManager.Teams.Client.Extensions;
using PoC.VirtualManager.Brain.FrontalLobe.Personality;
using PoC.VirtualManager.Brain.TemporalLobe;
using PoC.VirtualManager.Interactions.Infrastructure.Extensions;
using Azure.AI.TextAnalytics;
using Azure;
using PoC.VirtualManager.Playground.Extensions;
using PoC.VirtualManager.Models;
using PoC.VirtualManager.Interactions.Slack.Listener.Models;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, configuration) =>
    {
        var appsettingsName = "appsettings.json";
#if DEBUG
        appsettingsName = "appsettings.playground.json";
#endif
        configuration.AddJsonFile(appsettingsName, optional: true, reloadOnChange: true);
    })
    .ConfigureServices((Action<HostBuilderContext, IServiceCollection>)((context, services) =>
    {
        var slackListenerSettings = context.Configuration.GetSection("SlackListenerSettings").Get<SlackListenerSettings>();

        AddLogging(services);

        AddInteractionsInfrastructure(services);
        //TODO Add Teams infrastructure

        AddAIServicesWithPlugins(services);
        AddStreamWriterToFile(context, services);
#if DEBUG
        services.AddMockedClientsForApolloPlayground();
#else
        AddExternalClients(services);
#endif

        AddSlackIntegration(services, slackListenerSettings);
        AddGoogleMeetsIntegration(services);

        AddVirtualManagerPersonality(context, services);
        AddBrainLobes(services);
    }))
    .Build();

await host.RunAsync();

static void AddAIServicesWithPlugins(IServiceCollection services)
{
    var openAiKey = Environment.GetEnvironmentVariable("VirtualManager_OpenAI_ApiKey") ?? throw new Exception("VirtualManager_OpenAI_ApiKey env variable must be set");
    var langKey = Environment.GetEnvironmentVariable("VirtualManager_Azure_AI_LanguageKey") ?? throw new Exception("VirtualManager_Azure_AI_LanguageKey env variable must be set");
    var langEndpoint = Environment.GetEnvironmentVariable("VirtualManager_Azure_AI_LanguageEndpoint") ?? throw new Exception("VirtualManager_Azure_AI_LanguageEndpoint env variable must be set");
    Uri endpoint = new(langEndpoint);
    AzureKeyCredential credential = new(langKey);
    services.AddScoped<TextAnalyticsClient>(_ => new TextAnalyticsClient(endpoint, credential));

    #region Register Plugins 

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

static void AddStreamWriterToFile(HostBuilderContext context, IServiceCollection services)
{
    services.AddSingleton(_ =>
    {
        var writer = new StreamWriter(
            path: context.Configuration["FileDirectory"] + $"VirtualManager.txt", //_{DateTime.UtcNow.ToString("yyyyMMddTHHmm")}
            append: true
        );
        writer.AutoFlush = true;

        return writer;
    });
}

static void AddInteractionsInfrastructure(IServiceCollection services)
{
    var mongoDbConnectionString = Environment.GetEnvironmentVariable("VirtualManager_MongoDb_ConnectionString") ?? throw new Exception("VirtualManager_MongoDb_ConnectionString env variable must be set");
    services.AddInteractionsRepositories(mongoDbConnectionString);
}

static void AddGoogleMeetsIntegration(IServiceCollection services)
{
    //services.AddGoogleMeetsInteractionsListener();
}
static void AddSlackIntegration(IServiceCollection services, SlackListenerSettings settings)
{
    services.AddSlackInteractionsListener(settings);
    services.AddSlackFeedbackProvider();
}

static void AddExternalClients(IServiceCollection services)
{
    var slackAccessToken = Environment.GetEnvironmentVariable("VirtualManager_Slack_AccessToken") ?? throw new Exception("VirtualManager_Slack_AccessToken env variable must be set");
    services.AddTeamsClient();
    services.AddSlackClient(slackAccessToken);
}

static void AddBrainLobes(IServiceCollection services)
{
    //services.AddHostedService<PoC.VirtualManager.Brain.FrontalLobe.SlackProcessorBackgroundService>();
    services.AddHostedService<InteractionsEvaluatorBackgroundService>();
    services.AddHostedService<InteractionsSummarizerBackgroundService>();
}

static void AddVirtualManagerPersonality(HostBuilderContext context, IServiceCollection services)
{
    var personalitySection = context.Configuration.GetSection("Personality");
    var personality = personalitySection.Get<Personality>();

    services.AddSingleton(personality);
}

static void AddLogging(IServiceCollection services)
{
    services.AddLogging(l => l.AddConsole().SetMinimumLevel(LogLevel.Trace));
}