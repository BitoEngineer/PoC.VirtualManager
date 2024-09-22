using Microsoft.Extensions.DependencyInjection;
using PoC.VirtualManager.Interactions.Slack.Client.Extensions;
using FluentAssertions;
using PoC.VirtualManager.Slack.Client;
using PoC.VirtualManager.Interactions.Slack.Client;

namespace PoC.VirtualManager.Interactions.Slack.Listener.Tests
{
    [TestClass]
    public class SlackClientTests
    {
        [TestMethod]
        public async Task SendMessageAsyncTest()
        {
            //Arrange
            var client = GetSlackClient();
            var virtualManagerChannel = "C07K3CDME6R";

            //Act
            var result = await client.SendMessageAsync(virtualManagerChannel, nameof(SendMessageAsyncTest), default);

            //Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task ListChannelsAsyncTest()
        {
            //Arrange
            ISlackClient client = GetSlackClient();

            //Act
            var channels = await client.ListAllConversationsAsync(default(CancellationToken));

            //Assert
            channels.Should().NotBeNull();
            channels.Ok.Should().BeTrue();
        }

        [TestMethod]
        public async Task ListUsersConversationsAsyncTest()
        {
            //Arrange
            ISlackClient client = GetSlackClient();

            //Act
            var conversations = await client.ListUsersConversationsAsync(default(CancellationToken));

            //Assert
            conversations.Should().NotBeNull();
            conversations.Ok.Should().BeTrue();
        }

        [TestMethod]
        public async Task ListUsersAsyncTest()
        {
            //Arrange
            ISlackClient client = GetSlackClient();

            //Act
            var users = await client.ListUsersAsync(default(CancellationToken));

            //Assert
            users.Should().NotBeNull();
            users.Members.Any().Should().BeTrue();
        }

        [TestMethod]
        public async Task GetUserConversationHistoryAsyncTest()
        {
            //Arrange
            ISlackClient client = GetSlackClient();
            var allConversations = await client.ListAllConversationsAsync(default(CancellationToken));
            var conversation = (allConversations)
                .Conversations.OrderByDescending(x => x.Updated).First(x => x.IsIm);



            //Act
            foreach(var lala in allConversations.Conversations)
            {
                var members = await client.GetConversationMembersAsync(lala.Id, default);

                var to = DateTimeOffset.UtcNow;// DateTimeOffset.FromUnixTimeMilliseconds(lala.Updated);
                var from = to.AddDays(-1);
                var history = await client.GetConversationHistoryAsync(lala.Id, from, to, default);
                /*
                if (history.Contains("not_in_channel"))
                {
                    var yo = await client.JoinConversationAsync(lala.Id, default);
                }
                //If "{\"ok\":false,\"error\":\"not_in_channel\"}" - join the channel and try again

                //Assert
                history.Should().NotBeNull();*/
            }

        }

        private static ISlackClient GetSlackClient()
        {
            var accessToken = Environment.GetEnvironmentVariable("VirtualManager_Slack_AccessToken");
            var serviceProvider = new ServiceCollection()
                .AddSlackClient(accessToken)
                .BuildServiceProvider();

            var client = serviceProvider.GetRequiredService<ISlackClient>();
            return client;
        }
    }
}