using System;
using HarvestClient.ApiClients;

namespace HarvestClient
{
    public class HarvestRestClient
    {
        public HarvestRestClient(string bearerToken, int accountId)
        {
            if (bearerToken == null)
            {
                throw new ArgumentNullException(nameof(bearerToken));
            }

            Clients = new ClientApiClient(bearerToken, accountId);
            Projects = new ProjectApiClient(bearerToken, accountId);
            Users = new UserApiClient(bearerToken, accountId);
            Timesheets = new TimesheetApiClient(bearerToken, accountId);
        }

        public ClientApiClient Clients { get; }
        public ProjectApiClient Projects { get; }
        public UserApiClient Users { get; }
        public TimesheetApiClient Timesheets { get; }
    }

}

