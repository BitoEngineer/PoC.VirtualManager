using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Interactions.Slack.Client.Models
{

    public class ListUsersResponse
    {
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }

        [JsonPropertyName("members")]
        public List<Member> Members { get; set; }

        [JsonPropertyName("cache_ts")]
        public long CacheTs { get; set; }

        [JsonPropertyName("response_metadata")]
        public ResponseMetadata ResponseMetadata { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
    public class Profile
    {
        [JsonPropertyName("avatar_hash")]
        public string AvatarHash { get; set; }

        [JsonPropertyName("status_text")]
        public string StatusText { get; set; }

        [JsonPropertyName("status_emoji")]
        public string StatusEmoji { get; set; }

        [JsonPropertyName("real_name")]
        public string RealName { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("real_name_normalized")]
        public string RealNameNormalized { get; set; }

        [JsonPropertyName("display_name_normalized")]
        public string DisplayNameNormalized { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("image_24")]
        public string Image24 { get; set; }

        [JsonPropertyName("image_32")]
        public string Image32 { get; set; }

        [JsonPropertyName("image_48")]
        public string Image48 { get; set; }

        [JsonPropertyName("image_72")]
        public string Image72 { get; set; }

        [JsonPropertyName("image_192")]
        public string Image192 { get; set; }

        [JsonPropertyName("image_512")]
        public string Image512 { get; set; }

        [JsonPropertyName("image_1024")]
        public string Image1024 { get; set; }

        [JsonPropertyName("image_original")]
        public string ImageOriginal { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("skype")]
        public string Skype { get; set; }

        [JsonPropertyName("team")]
        public string Team { get; set; }
    }

    public class Member
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("team_id")]
        public string TeamId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("real_name")]
        public string RealName { get; set; }

        [JsonPropertyName("tz")]
        public string Tz { get; set; }

        [JsonPropertyName("tz_label")]
        public string TzLabel { get; set; }

        [JsonPropertyName("tz_offset")]
        public int TzOffset { get; set; }

        [JsonPropertyName("profile")]
        public Profile Profile { get; set; }

        [JsonPropertyName("is_admin")]
        public bool IsAdmin { get; set; }

        [JsonPropertyName("is_owner")]
        public bool IsOwner { get; set; }

        [JsonPropertyName("is_primary_owner")]
        public bool IsPrimaryOwner { get; set; }

        [JsonPropertyName("is_restricted")]
        public bool IsRestricted { get; set; }

        [JsonPropertyName("is_ultra_restricted")]
        public bool IsUltraRestricted { get; set; }

        [JsonPropertyName("is_bot")]
        public bool IsBot { get; set; }

        [JsonPropertyName("updated")]
        public long Updated { get; set; }

        [JsonPropertyName("is_app_user")]
        public bool IsAppUser { get; set; }

        [JsonPropertyName("has_2fa")]
        public bool Has2Fa { get; set; }
    }
}
