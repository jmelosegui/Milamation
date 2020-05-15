using System;
using System.Net;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using RestSharp;

namespace HarvestClient.Exceptions
{
    [Serializable]
    public class ApiException : Exception
    {
        public ApiException(IRestResponse response)
            : this(response, null)
        { }

        public ApiException(IRestResponse response, Exception innerException)
            : base(null, innerException)
        {
            StatusCode = response.StatusCode;
            ApiError = GetApiErrorFromExceptionMessage(response);
            HttpResponse = response;
        }

        /// <summary>
        /// Gets the HTTP status code associated with the repsonse
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        public IRestResponse HttpResponse { get; private set; }

        public override string Message
        {
            get { return ApiErrorMessageSafe ?? "An error occurred with this API request"; }
        }

        /// <summary>
        /// Gets the raw exception payload from the response
        /// </summary>
        public ApiError ApiError { get; private set; }

        /// <summary>
        /// Gets get the inner error message from the API response
        /// </summary>
        /// <remarks>
        /// Returns null if ApiError is not populated
        /// </remarks>
        protected string ApiErrorMessageSafe
        {
            get
            {
                if (ApiError != null && !string.IsNullOrWhiteSpace(ApiError.Message))
                {
                    return ApiError.Message;
                }

                return null;
            }
        }

        private static ApiError GetApiErrorFromExceptionMessage(IRestResponse response)
        {
            string responseBody = response != null ? response.Content as string : null;
            try
            {
                if (!string.IsNullOrEmpty(responseBody))
                {
                    return JsonConvert.DeserializeObject<ApiError>(responseBody);
                }
            }
            catch (Exception)
            {
                string title = Regex.Match(responseBody, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
                return new ApiError(title);
            }

            return new ApiError(responseBody);
        }
    }
}