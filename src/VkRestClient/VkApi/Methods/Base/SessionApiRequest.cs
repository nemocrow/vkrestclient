using Newtonsoft.Json.Linq;
using RestSharp;
using VkApi.Base;

namespace VkApi.Methods.Base
{
    public enum ApiRequestPriority
    {
        General,
        LongPoll,
        Auth
    }

    public interface ISessionApiRequest<T> : IApiRequest<T>
        where T : class
    {
        ApiRequestPriority Priority { get; }
    }

    public class SessionApiRequest<T> : ApiMethodRequest<T>, ISessionApiRequest<T>
        where T : class
    {
        private readonly ApiRequestPriority _priority;

        public SessionApiRequest(
            string methodName, 
            Method httpMethod, 
            ParameterDict parameterDict, 
            JsonReader<T> jsonReader = null,
            ApiRequestPriority priority = ApiRequestPriority.General)
            : base(methodName, httpMethod, parameterDict, jsonReader)
        {
            this._priority = priority;
        }

        public SessionApiRequest(
            IRestRequest restRequest, 
            JsonReader<T> jsonReader = null,
            ApiRequestPriority priority = ApiRequestPriority.General)
            : base(restRequest, jsonReader)
        {
            this._priority = priority;
        }

        public ApiRequestPriority Priority { get { return this._priority; } }
    }
}
