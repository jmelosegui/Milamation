using RestSharp;
using System;
using System.Diagnostics;
using System.Net;

namespace HarvestClient.Exceptions
{
    public class ForbiddenException : ApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// Constructs an instance of ForbiddenException
        /// </summary>
        /// <param name="response">The HTTP payload from the server</param>
        public ForbiddenException(IRestResponse response) : this(response, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// Constructs an instance of ForbiddenException
        /// </summary>
        /// <param name="response">The HTTP payload from the server</param>
        /// <param name="innerException">The inner exception</param>
        public ForbiddenException(IRestResponse response, Exception innerException)
            : base(response, innerException)
        {
            Debug.Assert(response != null && response.StatusCode == HttpStatusCode.Forbidden,
                "ForbiddenException created with wrong status code");
        }

        public override string Message
        {
            get { return "Request Forbidden"; }
        }
    }
}
