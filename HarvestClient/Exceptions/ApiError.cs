using System.Collections.Generic;

namespace HarvestClient.Exceptions
{
    public class ApiError
    {
        public ApiError(string message)
        {
            Message = message;
        }

        public int Code { get; set; }

        public string Message { get; set; }

        public IReadOnlyList<string> Errors { get; set; }
    }
}
