using System.Collections.Generic;
using Newtonsoft.Json;
using VkApi.Base;

namespace VkApi.JsonTypes
{
    public class VkChat : ApiData
    {
        [JsonProperty("users")]
        public IEnumerable<string> UserIds { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("chat_id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("admin_id")]
        public string AdminUserId { get; set; }
    }
}
