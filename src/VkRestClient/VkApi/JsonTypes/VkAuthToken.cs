using System;
using Newtonsoft.Json;
using VkApi.Base;

namespace VkApi.JsonTypes
{
    /// <summary>
    /// Вместе с ключом access_token также будет указано время его жизни expires_in, 
    /// заданное в секундах. Если срок использования ключа истек, то необходимо 
    /// повторно провести все описанные выше шаги, но в этом случае пользователю уже 
    /// не придется дважды разрешать доступ. Запрашивать access_token также необходимо 
    /// при смене пользователем логина или пароля или удалением приложения в настройках 
    /// доступа. 
    /// 
    /// Кроме того, среди возвращаемых параметров будет указан user_id - идентификатор 
    /// авторизовавшегося пользователя в социальной сети. 
    /// </summary>
    public class VkAuthToken : ApiData
    {
        [JsonProperty("access_token")]
        public string Value { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresInSeconds { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        public DateTime CreationTime { get; set; }
    }

}
