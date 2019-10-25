using HarvestClient.Exceptions;
using HarvestClient.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HarvestClient.ApiClients
{
    public class BaseApiClient
    {
        protected const string BaseUrl = "https://api.harvestapp.com/api/v2/";
        protected readonly string bearerToken;
        protected readonly string accountId;
        protected readonly JsonHttpPipeline jsonPipeline;

        private static readonly Dictionary<HttpStatusCode, Func<IRestResponse, Exception>> HttpExceptionMap =
            new Dictionary<HttpStatusCode, Func<IRestResponse, Exception>>
            {
                { HttpStatusCode.Unauthorized, response => new AuthorizationException(response) },
                { HttpStatusCode.Forbidden, response => new ForbiddenException(response) },
                { HttpStatusCode.NotFound, response => new NotFoundException(response) }
            };

        public BaseApiClient(string bearerToken, string accountId)
        {
            this.bearerToken = bearerToken ?? throw new ArgumentNullException(nameof(bearerToken));
            this.accountId = accountId;
            jsonPipeline = new JsonHttpPipeline();
        }

        protected async Task<T> ProcessRequest<T>(string endPoint, Method method, IDictionary<string, object> parameters = null, object body = null)
            where T : new()
        {
            string url = endPoint.StartsWith("http") ? endPoint : $"{BaseUrl}{endPoint}";

            var client = new RestClient(url);

            var request = new RestRequest(method);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Authorization", $"Bearer {this.bearerToken}");
            request.AddHeader("Harvest-Account-ID", $"{this.accountId}");

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    request.AddQueryParameter(parameter.Key, parameter.Value?.ToString());
                }
            }

            if (body != null)
            {
                request.AddJsonBody(body);
            }

            IRestResponse response = await client.ExecuteTaskAsync(request);

            HandleErrors(response);

            var result = jsonPipeline.DeserializeResponse<T>(response);

            return result;
        }

        private void HandleErrors(IRestResponse response)
        {
            Exception ex = null;
            Func<IRestResponse, Exception> exceptionFunc;
            if (HttpExceptionMap.TryGetValue(response.StatusCode, out exceptionFunc))
            {
                ex = exceptionFunc(response);
            }
            else if ((int)response.StatusCode >= 400)
            {
                ex = new ApiException(response);
            }
            else if (response.ErrorException != null)
            {
                ex = response.ErrorException;
            }

            if (ex != null)
            {
                throw ex;
            }
        }
    }
}
