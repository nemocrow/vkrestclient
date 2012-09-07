using Newtonsoft.Json.Linq;
using RestSharp;
using VkApi.Base;

namespace VkApi.Methods.Base
{
    public class ApiMethodRequest<T> : ApiRequest<T>
        where T : class
    {
        private static JToken ExtractResponse(JToken json)
        {
            // see if we have a success response
            if (json["response"] != null)
                return json["response"];
            else
                // otherwise, it's just some shit and we can't do anything about it.
                throw new UnknownJsonFormatException(json);
        }

        public static JsonReader<U> MethodResponseReader<U>(JsonReader<U> reader)
        {
            return json => reader(ExtractResponse(json));
        }

        public ApiMethodRequest(
            string methodName,
            Method httpMethod,
            ParameterDict parameterDict,
            JsonReader<T> jsonReader = null) :
            this(
                CreateRestRequest(httpMethod, string.Format("method/{0}", methodName), parameterDict),
                jsonReader)
        {
        }

        public ApiMethodRequest(
            IRestRequest restRequest,
            JsonReader<T> jsonReader = null) :
            base(
                restRequest,
                // wrap that shit
                MethodResponseReader(jsonReader ?? PlainReader))
        {
        }

    }
}
