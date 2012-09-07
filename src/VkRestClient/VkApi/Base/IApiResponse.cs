using System;

namespace VkApi.Base
{
    /// <summary>
    /// Represents response result from VKApi
    /// </summary>
    public interface IApiResponse
    {
        ApiError ApiError { get; }
        bool IsApiError { get; }
        bool IsSuccess { get; }
        bool IsException { get; }
        bool IsCaptchaRequired { get; }
        Exception Exception { get; }
        ApiCaptchaQuestion CaptchaQuestion { get; }
    }
}