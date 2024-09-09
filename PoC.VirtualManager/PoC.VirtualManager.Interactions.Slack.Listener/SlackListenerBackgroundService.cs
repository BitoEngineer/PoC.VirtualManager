using Microsoft.SemanticKernel.ChatCompletion;
using PoC.VirtualManager.Interactions.Models;
using PoC.VirtualManager.Interactions.Extensions;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.Logging;
using PoC.VirtualManager.Interactions.Slack.Listener.Caches;
using PoC.VirtualManager.Interactions.Slack.Listener.Extensions;
using System.Runtime.CompilerServices;
using PoC.VirtualManager.Slack.Client;
using PoC.VirtualManager.Interactions.Slack.Client.Models.Nested;
using PoC.VirtualManager.Slack.Client.Models.Messaging;

namespace PoC.VirtualManager.Interactions.Slack.Listener
{
    /* TODO:
     * - Get all messages since last iteration
     * - Slack control table - checkpoint per channel
     * - Create a private chat between the bot and each team member
     * TODO later:
     * - Listen to events via webhook (https://api.slack.com/apis/events-api, https://api.slack.com/apps/A07JRN1UU4X/event-subscriptions?)
     */
    internal class SlackListenerBackgroundService : BackgroundService
    {
        private readonly IChatCompletionService _chatCompletionService;
        private readonly StreamWriter _writer;
        private readonly ISlackClient _slackClient;
        private readonly IUsersCache _usersCache;
        private readonly ILogger<SlackListenerBackgroundService> _logger;
        private readonly Channel<SlackInteractionsQueueItem> _interactionsChannel;

        public SlackListenerBackgroundService(IChatCompletionService chatCompletionService,
            StreamWriter writer,
            ISlackClient slackClient,
            IUsersCache usersCache,
            ILogger<SlackListenerBackgroundService> logger,
            [FromKeyedServices("slack-interactions-channel")] Channel<SlackInteractionsQueueItem> interactionsChannel)
        {
            ArgumentNullException.ThrowIfNull(chatCompletionService, nameof(chatCompletionService));
            ArgumentNullException.ThrowIfNull(interactionsChannel, nameof(interactionsChannel));
            ArgumentNullException.ThrowIfNull(writer, nameof(writer));
            ArgumentNullException.ThrowIfNull(slackClient, nameof(slackClient));
            ArgumentNullException.ThrowIfNull(usersCache, nameof(usersCache));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            _chatCompletionService = chatCompletionService;
            _writer = writer;
            _interactionsChannel = interactionsChannel;
            _slackClient = slackClient;
            _usersCache = usersCache;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await foreach(var interaction in ConsumeLatestSlackInteractionsAsync(stoppingToken).WithCancellation(stoppingToken))
                {
                    if(interaction.ChannelName == "virtual-manager") //TODO remove
                    {
                        await _interactionsChannel.Writer.WriteAsync(interaction, stoppingToken);
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(60));
            }
        } 

        private async IAsyncEnumerable<SlackInteractionsQueueItem> ConsumeLatestSlackInteractionsAsync([EnumeratorCancellation] CancellationToken stoppingToken)
        {
            var allChannels = await _slackClient.ListAllConversationsAsync(stoppingToken);

            if (allChannels.Ok)
            {
                foreach (var channel in allChannels.Conversations) //DMs and private channels not included, and not possible :(
                {
                    var channelsInteractionsQueueItem = await ProcessSlackChannelAsync(channel, stoppingToken);
                    
                    if (!channelsInteractionsQueueItem.Messages.Any())
                    {
                        continue;
                    }

                    yield return channelsInteractionsQueueItem;
                }
            }
            else
            {
                _logger.LogWarning("Could not list Slack channels.");
            }
        }

        private async Task<SlackInteractionsQueueItem> ProcessSlackChannelAsync(Client.Models.Nested.Channel channel, CancellationToken stoppingToken)
        {
            var from = DateTimeOffset.UtcNow.AddDays(-15); //TODO control checkpoint 
            var to = DateTimeOffset.UtcNow; //TODO control checkpoint
            var history = await _slackClient.GetConversationHistoryAsync(
                channel.Id,
                from,
                to, 
                stoppingToken);

            if (history.Ok)
            {
                return await history.Messages.ToSlackInteractionsQueueItemAsync(
                    from,
                    to,
                    _usersCache,
                    channel, 
                    stoppingToken);
            }
            else if (history.Error.ToEnum() == SlackApiError.NotInChannel)
            {
                return await JoinChannelAndReprocessAsync(channel, stoppingToken);
            }
            else
            {
                _logger.LogWarning("Could not consume messages from Slack channel {ChannelId} due to {Error}", channel.Id, history.Error);
                return null;
            }
        }

        private async Task<SlackInteractionsQueueItem> JoinChannelAndReprocessAsync(Client.Models.Nested.Channel channel, CancellationToken stoppingToken)
        {
            var joinResponse = await _slackClient.JoinConversationAsync(channel.Id, stoppingToken);

            if (joinResponse.Ok)
            {
                _logger.LogInformation("Joined channel {ChannelId}", channel.Id);
                return await ProcessSlackChannelAsync(channel, stoppingToken);
            }
            else
            {
                _logger.LogError("Could not join channel {ChannelId} due to {Error}", channel.Id, joinResponse.Error);
            }

            return new SlackInteractionsQueueItem
            {
                Messages = new List<SlackMessage>()
            };
        }


        private async Task<string> GetMockedInteractionAsPlainTextAsync()
        {
            var promptToGenerateInteraction = await File.ReadAllTextAsync("Prompts/GenerateChatInteractionsPromptPlainText.txt");
            var interactionStream = _chatCompletionService.GetStreamingChatMessageContentsAsync(promptToGenerateInteraction);
            var fullInteraction = new StringBuilder();

            _writer.WriteLine();
            _writer.WriteLine("--------------------------------------------------------------------------------------------");
            _writer.WriteLine("Team > ");
            await foreach (var tokens in interactionStream)
            {
                _writer.Write(tokens.Content);
                fullInteraction.Append(tokens.Content);
            }

            return fullInteraction.ToString();
        }

        private async Task<(string Json, IInteraction Object)> GetMockedInteractionAsJsonAsync(List<string> promptsKeys)
        {
            var promptToGenerateInteraction = await File.ReadAllTextAsync("Prompts/GenerateChatInteractionsPrompt.txt");
            var interactionJson = (await _chatCompletionService.GetChatMessageContentAsync(promptToGenerateInteraction)).Content;
            if (!interactionJson.TryDeserialize(typeof(ChatInteractions), out object interaction))
            {
                throw new Exception("Could not parse response: " + interactionJson);
            }

            return (interactionJson, interaction as IInteraction);
        }
    }
}
