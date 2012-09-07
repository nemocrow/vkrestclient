using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using RestSharp;
using VkApi.Base;

namespace VkApi.Methods.Base
{
    /// <summary>
    /// API method call with arbitraty IRestClient.
    /// </summary>
    /// <typeparam name="T">Type of response data.</typeparam>
    public interface IStandaloneApiRequest<T> : IApiRequest<T>
        where T : class
    {
        IRestClient RestClient { get; }
    }

    public class StandaloneApiRequest<T> : ApiRequest<T>, IStandaloneApiRequest<T>
        where T : class
    {
        private readonly IRestClient _restClient;

        public StandaloneApiRequest(
            IRestClient restClient,
            IRestRequest restRequest,
            JsonReader<T> jsonReader = null)
            : base(restRequest, jsonReader)
        {
            this._restClient = restClient;
        }

        public IRestClient RestClient { get { return this._restClient; } }
    }
}
