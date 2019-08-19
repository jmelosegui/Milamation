using RestSharp;
using System;
using System.Diagnostics;
using System.Net;

namespace HarvestClient.Exceptions
{
    public class NotFoundException : ApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// Constructs an instance of NotFoundException
        /// </summary>
        /// <param name="response">The HTTP payload from the server</param>
        public NotFoundException(IRestResponse response) : this(response, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// Constructs an instance of NotFoundException
        /// </summary>
        /// <param name="response">The HTTP payload from the server</param>
        /// <param name="innerException">The inner exception</param>
        public NotFoundException(IRestResponse response, Exception innerException)
            : base(response, innerException)
        {
            Debug.Assert(response != null && response.StatusCode == HttpStatusCode.NotFound,
                "NotFoundException created with wrong status code");
        }
    }
}
