using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using VkApi.Base;
using VkApi.JsonTypes;
using VkApi.Methods.Base;

namespace VkApi
{
    public class VkClient
    {
        public const string ApiBaseUrl = @"https://api.vk.com";

        public readonly VkApiSession Session;

        public VkClient(VkAuthToken authToken)
        {
            this.Session = new VkApiSession(ApiBaseUrl, authToken);
        }

        public VkClient(string accessToken)
            : this(new VkAuthToken { Value = accessToken }) {}

        public Task<ApiResponse<T>> Execute<T>(
            ISessionApiRequest<T> apiRequest)
            where T : class 
        {
            // @TODO prioritize!

            return Execute(apiRequest, this.Session.RestClient);
        }

        public static Task<ApiResponse<T>> Execute<T>(
            IStandaloneApiRequest<T> apiRequest)
            where T : class
        {
            return Execute(apiRequest, apiRequest.RestClient);
        }

        public static Task<ApiResponse<T>> Execute<T>(
            IApiRequest<T> apiRequest,
            IRestClient restClient)
            where T : class
        {
            DebugRequest(restClient, apiRequest.RestRequest);

            var t = new TaskCompletionSource<ApiResponse<T>>();
            restClient.ExecuteAsync(
                apiRequest.RestRequest,
                restResponse =>
                    {
                        t.SetResult(apiRequest.GetResponse(restResponse));
                    }
            );

            return t.Task;
        }

        private static void DebugRequest(IRestClient client, IRestRequest request)
        {
            Debug.WriteLine(
                "Executing: {0}/{1}: {2}",
                client.BaseUrl,
                request.Resource,
                request.Parameters.Aggregate("", (head, param) => string.Format("{0} '{1}:{2}',", head, param.Name, param.Value)));
        }
    }
}
