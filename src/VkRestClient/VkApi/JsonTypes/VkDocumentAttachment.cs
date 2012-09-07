using Newtonsoft.Json;

namespace VkApi.JsonTypes
{
    /// <summary>
    /// Document (type = doc)
    /// </summary>
    public class VkDocumentAttachment : VkBaseAttachment
    {
        /// <summary>
        /// did document identifier
        /// </summary>
        [JsonProperty("did")]
        public string Id { get; set; }

        /// <summary>
        /// ext document format (file extension)
        /// </summary>
        [JsonProperty("ext")]
        public string DocumentFormat { get; set; }

        /// <summary>
        /// url URL for loading the document
        /// </summary>
        [JsonProperty("url")]
        public string DocumentUrl { get; set; }

        /// <summary>
        /// title document title
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// size document size
        /// </summary>
        [JsonProperty("size")]
        public int DocumentSize { get; set; }
    }
}