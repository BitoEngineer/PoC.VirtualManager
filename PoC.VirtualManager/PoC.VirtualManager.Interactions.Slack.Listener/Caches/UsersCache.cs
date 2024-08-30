using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PoC.VirtualManager.Interactions.Slack.Client.Models;
using PoC.VirtualManager.Slack.Client;

namespace PoC.VirtualManager.Interactions.Slack.Listener.Caches
{
    public interface IUsersCache
    {
        Task<Member> GetUserAsync(string userId, CancellationToken cancellationToken);
    }

    public class UsersCache : IUsersCache
    {
        private readonly ISlackClient _slackClient;
        private readonly ILogger<UsersCache> _logger;
        private readonly MemoryCache _cache;

        private readonly MemoryCacheEntryOptions _cacheEntryOptions = new MemoryCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromDays(1),
        };

        public UsersCache(
            ISlackClient slackClient,
            ILogger<UsersCache> logger)
        {
            ArgumentNullException.ThrowIfNull(slackClient, nameof(slackClient));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            _slackClient = slackClient;
            _logger = logger;
            _cache = new MemoryCache(new MemoryCacheOptions
            {
            });
        }

        public async Task<Member> GetUserAsync(string userId, CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(userId, out object member))
            {
                member = await LoadCacheAsync(userId, cancellationToken);
            }

            return member as Member;
        }

        private async Task<Member> LoadCacheAsync(string userId, CancellationToken cancellationToken)
        {
            Member member = new Member
            {
                Id = userId,
                Name = "Unkown user",
                RealName = "Unkown user"
            };

            var allMembers = await _slackClient.ListUsersAsync(cancellationToken);
            if (allMembers.Ok)
            {
                allMembers.Members.ForEach(item =>
                {
                    _cache.Set(item.Id, item);
                    if (item.Id == userId)
                    {
                        member = item;
                    }
                });

                if(!allMembers.Members.Any(m => m.Id == userId))
                {
                    _cache.Set(userId, member);
                }
            }
            else
            {
                throw new Exception($"Could not find user {userId} - {allMembers.Error}");
            }

            return member;
        }
    }
}
