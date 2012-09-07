using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VkApi.Base
{
    [JsonObject]
    public class ApiData
    {
        public JToken RawJson { get; set; }

        public static T FromJson<T>(JToken json)
            where T : class
        {
            var o = json.ToObject<T>();

            if (o is ApiData)
            {
                (o as ApiData).RawJson = json;
            }

            return o;
        }
    }
}
