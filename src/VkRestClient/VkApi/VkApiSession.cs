using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RestSharp;
using VkApi.JsonTypes;

namespace VkApi
{   
    /// <summary>
    /// Vk REST API client session.
    /// 
    /// After you authenticate, you get the access token, which you can use to create
    /// an instance of VkClient, which in turn can execute API requests for you. 
    /// </summary>
    public class VkApiSession
    {
        public readonly VkAuthToken AuthToken;
        public readonly IRestClient RestClient;

        /// <summary>
        /// Will add "&access_token=(_accessToken)" to each request. That's how it's done with RestSharp.
        /// </summary>
        private class AccessTokenAuthenticator : IAuthenticator
        {
            private readonly string _accessToken;

            public AccessTokenAuthenticator(string accessToken)
            {
                this._accessToken = accessToken;
            }

            public void Authenticate(IRestClient client, IRestRequest request)
            {
                request.AddParameter("access_token", this._accessToken);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="apiBaseUrl">API base url. https://api.vk.com/. </param>
        /// <param name="authToken">Access token you receive after successful authentication.</param>
        public VkApiSession(string apiBaseUrl, VkAuthToken authToken)
        {
            this.AuthToken = authToken;

            this.RestClient = new RestClient(apiBaseUrl)
                { Authenticator = new AccessTokenAuthenticator(this.AuthToken.Value) };
        }
    }
}
