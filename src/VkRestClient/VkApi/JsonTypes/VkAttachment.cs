using Newtonsoft.Json;
using VkApi.Base;

namespace VkApi.JsonTypes
{
    /// <summary>
    /// If media content is attached to a message, the object "message" will also 
    /// contain the field "attachments", representing an array of objects-attachments. 
    /// Each of these objects has two fields: "type", the value of which is equal to 
    /// "photo", "audio", "video" or "doc", and represents the type of the attachment, 
    /// as well as a second field that matches the name with the value "type". The 
    /// value of the second field will be the object that describes attachment 
    /// parameters. Its collection of fields will be different depending on the type 
    /// of the attachment. 
    /// </summary>
    public class VkAttachment : ApiData
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("photo")]
        public VkPhotoAttachment Photo { get; set; }

        [JsonProperty("audio")]
        public VkAudioAttachment Audio { get; set; }

        [JsonProperty("video")]
        public VkVideoAttachment Video { get; set; }

        [JsonProperty("document")]
        public VkDocumentAttachment Document { get; set; }

        // govno
        public string OwnerId
        {
            get
            {
                return (Type == "photo" ? Photo.OwnerId :
                       (Type == "audio" ? Audio.OwnerId :
                       (Type == "video" ? Video.OwnerId :
                       (Type == "document" ? Document.OwnerId : null))));
            }
        }

        // kal
        public string MediaId
        {
            get
            {
                return (Type == "photo" ? Photo.Id :
                       (Type == "audio" ? Audio.Id :
                       (Type == "video" ? Video.Id :
                       (Type == "document" ? Document.Id : null))));
            }
        }
    }
}