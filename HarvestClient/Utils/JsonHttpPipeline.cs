using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;

namespace HarvestClient.Utils
{
    /// <summary>
    ///     Responsible for serializing the request and response as JSON and
    ///     adding the proper JSON response header.
    /// </summary>
    public class JsonHttpPipeline
    {
        private readonly JsonSerializerSettings settings;

        public JsonHttpPipeline()
            : this(new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })
        {
        }

        public JsonHttpPipeline(JsonSerializerSettings settings)
        {
            this.settings = settings;
        }

        public T DeserializeResponse<T>(IRestResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (response.ContentType != null && response.ContentType.Equals("application/json; charset=utf-8", StringComparison.Ordinal))
            {
                var body = response.Content as string;

                if (typeof(T) != typeof(string) && !string.IsNullOrEmpty(body) && body != "{}")
                {
                    var result = JsonConvert.DeserializeObject<T>(body, settings);

                    return result;
                }
            }

            return default;
        }
    }
}
