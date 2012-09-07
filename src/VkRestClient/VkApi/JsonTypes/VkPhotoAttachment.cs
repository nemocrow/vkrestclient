using Newtonsoft.Json;

namespace VkApi.JsonTypes
{
    /// <summary>
    /// Photo (type = photo)
    /// pid photo identifier
    /// src image URL
    /// src_big maximized version URL
    /// </summary>
    public class VkPhotoAttachment : VkBaseAttachment
    {
        [JsonProperty("pid")]
        public string Id { get; set; }

        [JsonProperty("src")]
        public string Src { get; set; }

        [JsonProperty("src_big")]
        public string SrcBig { get; set; }
    }
}