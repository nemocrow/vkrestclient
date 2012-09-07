using System;
using Newtonsoft.Json.Linq;

namespace VkApi.Base
{
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message)
        {
        }
    }

    public class ApiErrorException : ApiException
    {
        public ApiErrorException(ApiError error)
            : base(string.Format("API method returned an error: {0}, {1}", error.ErrorCode, error.ExtendedError))
        {
            this.ApiError = error;
        }

        public readonly ApiError ApiError;
    }

    public class ApiCaptchaRequiredException : ApiException
    {
        public ApiCaptchaRequiredException(ApiCaptchaQuestion captchaQuestion)
            : base(string.Format("API method call replied with \"need_captcha\"."))
        {
            this.CaptchaQuestion = captchaQuestion;
        }

        public readonly ApiCaptchaQuestion CaptchaQuestion;
    }

    public class JsonException : ApiException
    {
        public readonly JToken OriginalJson;
        public readonly Exception ActualException;

        public JsonException(
            Exception actualException = null, 
            JToken json = null, 
            string message = "JSON manipulation failed.") :
            base(message)
        {
            this.OriginalJson = json;
            this.ActualException = actualException;
        }
    }

    public class UnknownJsonFormatException : JsonException
    {
        public UnknownJsonFormatException(JToken json, Exception actual = null) : 
            base(actual, json, string.Format("Unknown JSON data format: {0}", json.ToString()))
        {
        }
    }

    public class JsonMiddlewareException : JsonException
    {
        public JsonMiddlewareException(JToken json, Exception actual) :
            base(actual, json, string.Format("Could not read JSON data: {0}", actual.Message))
        {
        }
    }
}
