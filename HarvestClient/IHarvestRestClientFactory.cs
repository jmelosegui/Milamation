namespace HarvestClient
{
    public interface IHarvestRestClientFactory
    {
        IHarvestRestClient CreateHarvestRestClient(string bearerToken, string accountId);
    }

    public class HarvestRestClientFactory : IHarvestRestClientFactory
    {
        public IHarvestRestClient CreateHarvestRestClient(string bearerToken, string accountId)
        {
            return new HarvestRestClient(bearerToken, accountId);
        }
    }
}