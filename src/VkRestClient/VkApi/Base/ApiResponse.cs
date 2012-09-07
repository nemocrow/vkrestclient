using System;
using VkApi.Methods.Base;

namespace VkApi.Base
{
    /// <summary>
    /// A response from API call.
    /// 
    /// There are four kinds of response: success, error, exception and captcharequired.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T> : IApiResponse where T : class
    {
        public readonly ApiRequest<T> Request;

        private readonly T _data;
        private readonly Exception _exception;
        private readonly ApiCaptchaQuestion _captchaQuestion;
        
        public ApiError ApiError
        {
            get
            {
                return (this._exception as ApiErrorException) != null ? (this._exception as ApiErrorException).ApiError : null;
            }
        }
        
        public bool IsApiError { get { return this._exception != null && this._exception is ApiErrorException; } }
        public bool IsSuccess { get { return this._data != null; } }
        public bool IsException { get { return this._exception != null; } }
        public bool IsCaptchaRequired { get { return this._captchaQuestion != null; } }
        public Exception Exception { get { return _exception; } }
        public ApiCaptchaQuestion CaptchaQuestion { get { return _captchaQuestion; } }
        
        public T Data { get { return _data; } }

        private ApiResponse(ApiRequest<T> request, T responseData, Exception e, ApiCaptchaQuestion captchaQuestion)
        {
            this.Request = request;
            this._data = responseData;
            this._exception = e;
            this._captchaQuestion = captchaQuestion;
        }

        /// <summary>
        /// Creates a success response. Use this when your API call succeeded and you
        /// have some data to return.
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        public static ApiResponse<T> Success(ApiRequest<T> request, T responseData)
        {
            return new ApiResponse<T>(request, responseData, null, null);
        }

        /// <summary>
        /// Creates exception response. Use this when an API call failed.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static ApiResponse<T> Failure(ApiRequest<T> request, Exception e)
        {
            return new ApiResponse<T>(request, null, e, null);
        }
    }
}
