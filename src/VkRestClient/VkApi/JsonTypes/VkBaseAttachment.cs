using Newtonsoft.Json;
using VkApi.Base;

namespace VkApi.JsonTypes
{
    public class VkBaseAttachment : ApiData
    {
        /// <summary>
        /// owner_id document owner identifier
        /// owner_id photo owner identifier
        /// owner_id audio file owner identifier
        /// owner_id video owner identifier
        /// </summary>
        [JsonProperty("owner_id")]
        public string OwnerId { get; set; }
    }
}