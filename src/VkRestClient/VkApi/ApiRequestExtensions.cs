using System;
using System.Threading.Tasks;
using System.Windows;
using RestSharp;
using VkApi;
using VkApi.Base;
using VkApi.Methods.Base;

namespace VkApi
{
    /// <summary>
    /// Extensions for convenient usage of IApiRequests in UI environment.
    /// </summary>
    public static class ApiRequestExtensions
    {
        /// <summary>
        /// Executes a standalone API method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiRequest"></param>
        /// <returns></returns>
        public static Task<ApiResponse<T>> Execute<T>(
            this IStandaloneApiRequest<T> apiRequest)
            where T : class
        {
            return VkClient.Execute(apiRequest);
        }

        /// <summary>
        /// Executes API method in a session context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiRequest"></param>
        /// <param name="sessionContext"></param>
        /// <returns></returns>
        public static Task<ApiResponse<T>> ExecuteIn<T>(
            this ISessionApiRequest<T> apiRequest,
            VkClient sessionContext)
            where T : class
        {
            return sessionContext.Execute(apiRequest);
        }

        /// <summary>
        /// Executes one of the "special" auth methods.
        /// Fuck Vk.
        /// 
        /// This is a sessionless call, meaning that authorization token is not
        /// passed as a parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiRequest"></param>
        /// <param name="apiBaseUrl"></param>
        /// <returns></returns>
        public static Task<ApiResponse<T>> ExecuteFree<T>(
            this IApiRequest<T> apiRequest,
            string apiBaseUrl = VkClient.ApiBaseUrl)
            where T : class
        {
            return VkClient.Execute(
                apiRequest,
                new RestClient(apiBaseUrl)
            );
        }
    }
}
