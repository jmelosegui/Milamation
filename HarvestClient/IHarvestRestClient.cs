namespace HarvestClient
{
    public interface IHarvestRestClient
    {
        ApiClients.ClientApiClient Clients { get; }
        ApiClients.ProjectApiClient Projects { get; }
        ApiClients.UserApiClient Users { get; }
        ApiClients.TimesheetApiClient Timesheets { get; }
    }
}