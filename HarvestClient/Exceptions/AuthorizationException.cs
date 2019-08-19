using RestSharp;
using System;
using System.Diagnostics;
using System.Net;

namespace HarvestClient.Exceptions
{
    public class AuthorizationException : ApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationException"/> class.
        /// Constructs an instance of AuthorizationException
        /// </summary>
        /// <param name="response">The HTTP payload from the server</param>
        public AuthorizationException(IRestResponse response)
            : this(response, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationException"/> class.
        /// Constructs an instance of AuthorizationException
        /// </summary>
        /// <param name="response">The HTTP payload from the server</param>
        /// <param name="innerException">The inner exception</param>
        public AuthorizationException(IRestResponse response, Exception innerException)
            : base(response, innerException)
        {
            Debug.Assert(response != null && response.StatusCode == HttpStatusCode.Unauthorized,
                "AuthorizationException created with wrong status code");
        }

        public override string Message
        {
            get { return "Token is invalid or expired"; }
        }
    }
}
