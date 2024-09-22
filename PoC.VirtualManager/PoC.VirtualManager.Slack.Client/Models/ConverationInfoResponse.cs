using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Slack.Client.Models
{
    public class ConversationInfoResponse
    {
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }

        [JsonPropertyName("channel")]
        public Channel Channel { get; set; }
    }

    public class Channel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("is_archived")]
        public bool IsArchived { get; set; }

        [JsonPropertyName("is_im")]
        public bool IsIm { get; set; }

        [JsonPropertyName("is_org_shared")]
        public bool IsOrgShared { get; set; }

        [JsonPropertyName("context_team_id")]
        public string ContextTeamId { get; set; }

        [JsonPropertyName("updated")]
        public long Updated { get; set; }

        [JsonPropertyName("user")]
        public string User { get; set; }

        [JsonPropertyName("last_read")]
        public string LastRead { get; set; }

        [JsonPropertyName("latest")]
        public LatestMessage Latest { get; set; }

        [JsonPropertyName("team")]
        public string Team { get; set; }

        [JsonPropertyName("bot_profile")]
        public BotProfile BotProfile { get; set; }

        [JsonPropertyName("blocks")]
        public List<Block> Blocks { get; set; }

        [JsonPropertyName("unread_count")]
        public int UnreadCount { get; set; }

        [JsonPropertyName("unread_count_display")]
        public int UnreadCountDisplay { get; set; }

        [JsonPropertyName("is_open")]
        public bool IsOpen { get; set; }

        [JsonPropertyName("priority")]
        public int Priority { get; set; }

        [JsonPropertyName("num_members")]
        public int NumMembers { get; set; }
    }

    public class LatestMessage
    {
        [JsonPropertyName("user")]
        public string User { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("ts")]
        public string Ts { get; set; }

        [JsonPropertyName("bot_id")]
        public string BotId { get; set; }

        [JsonPropertyName("app_id")]
        public string AppId { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class BotProfile
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("updated")]
        public long Updated { get; set; }

        [JsonPropertyName("app_id")]
        public string AppId { get; set; }

        [JsonPropertyName("icons")]
        public Icons Icons { get; set; }

        [JsonPropertyName("team_id")]
        public string TeamId { get; set; }
    }

    public class Icons
    {
        [JsonPropertyName("image_36")]
        public string Image36 { get; set; }

        [JsonPropertyName("image_48")]
        public string Image48 { get; set; }

        [JsonPropertyName("image_72")]
        public string Image72 { get; set; }
    }

    public class Block
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("block_id")]
        public string BlockId { get; set; }

        [JsonPropertyName("elements")]
        public List<Element> Elements { get; set; }
    }

    public class Element
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("elements")]
        public List<RichTextElement> Elements { get; set; }
    }

    public class RichTextElement
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
