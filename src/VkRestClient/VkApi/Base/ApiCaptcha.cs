using Newtonsoft.Json;

namespace VkApi.Base
{
    /// <summary>
    /// Returned by an API call when CAPTCHA was requested by the server.
    /// </summary>
    public class ApiCaptchaQuestion : ApiData
    {
        [JsonProperty("captcha_sid")]
        public string CaptchaId { get; set; }

        [JsonProperty("capthca_img")]
        public string CaptchaImageUri { get; set; }
    }

    /// <summary>
    /// Pass this to an API call to send the captcha key response from the user.
    /// </summary>
    public class ApiCaptchaAnswer
    {
        public readonly ApiCaptchaQuestion CaptchaQuestion;
        public readonly string UserResponse;

        public ApiCaptchaAnswer(ApiCaptchaQuestion captchaQuestion, string response)
        {
            this.CaptchaQuestion = captchaQuestion;
            this.UserResponse = response;
        }
    }
}
