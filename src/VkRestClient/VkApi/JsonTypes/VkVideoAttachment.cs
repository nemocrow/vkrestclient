using Newtonsoft.Json;

namespace VkApi.JsonTypes
{
    /// <summary>
    /// Video (type = video)
    /// </summary>
    public class VkVideoAttachment : VkBaseAttachment
    {
        /// <summary>
        /// vid video identifier
        /// </summary>
        [JsonProperty("vid")]
        public string Id { get; set; }

        /// <summary>
        /// title title
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// duration video duration (in seconds)
        /// </summary>
        [JsonProperty("duration")]
        public int DurationSeconds { get; set; }
    }
}