using HarvestClient.Model;
using HarvestClient.Utils;
using RestSharp;
using System;
using System.Collections.Generic;

namespace HarvestClient.ApiClients
{
    public class TimesheetApiClient : BaseApiClient
    {
        public TimesheetApiClient(string bearerToken, string accountId) 
            : base(bearerToken, accountId)
        {
        }

        public async IAsyncEnumerable<TimesheetEntry> ListAsync(int? clientId, int? projectId, DateTime? startDate, DateTime? endDate, int? userId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            int currentPage = 1;
            FluentDictionary.For(parameters)
                            .Add("client_id", clientId, () => clientId.HasValue)
                            .Add("project_id", projectId, () => projectId.HasValue)
                            .Add("user_id", userId, () => userId.HasValue)
                            .Add("from", startDate?.ToString("yyyy-MM-dd"), () => startDate.HasValue)
                            .Add("to", endDate?.ToString("yyyy-MM-dd"), () => endDate.HasValue)
                            .Add("page", currentPage); ;

            var collection = await ProcessRequest<TimeEntryCollection>("time_entries", Method.GET, parameters);

            while (collection.TotalPages >= currentPage)
            {
                foreach (var project in collection.TimeEntries)
                {
                    yield return project;
                }

                parameters["page"] = ++currentPage;
                collection = await ProcessRequest<TimeEntryCollection>("time_entries", Method.GET, parameters);
            }
        }
    }
}
