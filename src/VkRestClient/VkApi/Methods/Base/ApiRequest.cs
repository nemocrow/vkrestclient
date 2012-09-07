using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using VkApi.Base;

namespace VkApi.Methods.Base
{
    /// <summary>
    /// An API method call object. Instantiated with method call parameters,
    /// and is used to perform a call to specific API method.
    /// </summary>
    /// <typeparam name="T">Response data type.</typeparam>
    public interface IApiRequest<T> where T : class
    {
        IRestRequest RestRequest { get; }

        /// <summary>
        /// Creates an ApiResponse from raw IRestResponse.
        /// </summary>
        /// <param name="restResponse"></param>
        /// <returns></returns>
        ApiResponse<T> GetResponse(IRestResponse restResponse);
    }

    public static class ApiRequestExtensions
    {
        /// <summary>
        /// Adds captcha answer parameters to a method call.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodCall"></param>
        /// <param name="captchaAnswer"></param>
        /// <returns></returns>
        public static IApiRequest<T> AddCaptchaAnswer<T>(
            this IApiRequest<T> methodCall,
            ApiCaptchaAnswer captchaAnswer)
            where T : class
        {
            // В этом случае необходимо повторить запрос, добавив к нему следующие параметры:
            // captcha_sid - поле captcha_sid, полученное в предыдущем запросе
            // captcha_key - то, что ввёл пользователь

            // @TODO what if called twice? need to check.
            methodCall.RestRequest.AddParameter("captcha_sid", captchaAnswer.CaptchaQuestion.CaptchaId);
            methodCall.RestRequest.AddParameter("captcha_key", captchaAnswer.UserResponse);

            return methodCall;
        }
    
    }

    public delegate T JsonReader<T>(JToken json);

    public class ParameterDict : Dictionary<string, object>
    {
        public static ParameterDict Empty = new ParameterDict {};
    }

    public class ApiRequest<T> : IApiRequest<T>
        where T : class
    {
        public IRestRequest RestRequest { get { return this._restRequest; } }

        private readonly IRestRequest _restRequest;
        private readonly JsonReader<T> _readJson;

        public static T PlainReader(JToken json)
        {
            return ApiData.FromJson<T>(json);
        }

        public ApiRequest(
            IRestRequest restRequest,
            JsonReader<T> jsonReader = null)
        {
            this._readJson = jsonReader ?? PlainReader;
            this._restRequest = restRequest;
        }

        public static IRestRequest AddRequestParameters(
            IRestRequest restRequest,
            ParameterDict parameters)
        {
            foreach (var item in parameters.Where(item => item.Value != null)) {
                restRequest.AddParameter(item.Key, item.Value.ToString());
            }

            return restRequest;
        }

        public static IRestRequest CreateRestRequest(
            Method requestMethod, string resource,
            ParameterDict parameters)
        {
            return AddRequestParameters(
                new RestRequest(resource, requestMethod),
                parameters);
        }


        public ApiRequest(
            string resource,
            Method httpMethod,
            ParameterDict parameters,
            JsonReader<T> jsonReader = null) :
            this(
                CreateRestRequest(httpMethod, resource, parameters),
                jsonReader)
        {
        }

        public ApiResponse<T> GetResponse(IRestResponse restResponse)
        {
            // possible cases of such status:
            // 1) HTTP transport error
            // 2) any other exception
            // this is a request error.
            if (restResponse.ResponseStatus != ResponseStatus.Completed)
                return ApiResponse<T>.Failure(this, restResponse.ErrorException);
            else
            {
                // no transport errors -- try to parse the json from response
                try
                {
                    var jsonRoot = JsonConvert.DeserializeObject<JObject>(restResponse.Content);

                    var errorNode = jsonRoot["error"];
                    // this is an error response
                    if (errorNode != null)
                    {
                        // this is a regular error response.
                        // {"error": {"error_code":5, "error_msg":"User authorization failed: user revoke access for this token.", "request_params":[ ...
                        if (errorNode is JObject)
                        {
                            var apiError = ApiData.FromJson<ApiError>(errorNode);

                            if (apiError.ErrorCode == ErrorCode.CaptchaRequired)
                            {
                                //Если возникает ошибка "Captcha is needed", то в стандартном сообщении об ошибке передаются также следующие параметры: 
                                //captcha_sid - идентификатор captcha
                                //captcha_img - ссылка на изображение, которое нужно показать пользователю, 
                                //чтобы он ввел текст с этого изображения.
                                throw new ApiCaptchaRequiredException(ApiData.FromJson<ApiCaptchaQuestion>(errorNode));
                            }
                            else
                                throw new ApiErrorException(apiError);
                        }
                        else
                        {
                            // every API call can return a "captcha required" error.
                            // {"error":"need_captcha","captcha_sid":"854844498568","captcha_img":"http:\/\/api.vk.com\/captcha.php?sid=854844498568&s=1"}
                            if (errorNode.ToString() == "need_captcha")
                                throw new ApiCaptchaRequiredException(ApiData.FromJson<ApiCaptchaQuestion>(jsonRoot));
                            // this is some other fucked up error
                            else
                                throw new ApiErrorException(ApiData.FromJson<ApiError>(jsonRoot));
                        }
                    }
                    else
                    {
                        try
                        {
                            return ApiResponse<T>.Success(this, this._readJson(jsonRoot));
                        }
                        catch (Exception e)
                        {
                            throw new JsonMiddlewareException(jsonRoot, e);
                        }
                    }
                }
                // somehow we couldn't get a json-object from our response. this classifies
                // as an exception.
                catch (Exception e)
                {
                    return ApiResponse<T>.Failure(this, e);
                }
            }
        }
    }
}
