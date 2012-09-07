using Newtonsoft.Json;

namespace VkApi.JsonTypes
{
    /// <summary>
    /// Audio File (type = audio)
    /// </summary>
    public class VkAudioAttachment : VkBaseAttachment
    {
        /// <summary>
        /// aid audio file identifier
        /// </summary>
        [JsonProperty("aid")]
        public string Id { get; set; }

        /// <summary>
        /// performer audio file performer
        /// </summary>
        [JsonProperty("performer")]
        public string Performer { get; set; }

        /// <summary>
        /// title audio file title
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// duration audio file duration (in seconds)
        /// </summary>
        [JsonProperty("duration")]
        public int DurationSeconds { get; set; }
    }
}