using HarvestClient.Model;
using HarvestClient.Utils;
using RestSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarvestClient.ApiClients
{
    public class ClientApiClient : BaseApiClient
    {
        public ClientApiClient(string bearerToken, string accountId)
            : base(bearerToken, accountId)
        {
        }

        public async Task<Client> GetById(int id)
        {
            return await ProcessRequest<Client>($"clients/{id}", Method.GET);
        }

        public async IAsyncEnumerable<Client> ListAsync(bool? isActive = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            int currentPage = 1;
            FluentDictionary.For(parameters)
                            .Add("is_active", isActive.HasValue ? isActive.ToString().ToLower() : null, () => isActive.HasValue)
                            .Add("page", currentPage);


            var collection = await ProcessRequest<ClientCollectionResponse>("clients", Method.GET, parameters);

            while (collection.TotalPages >= currentPage)
            {
                foreach (var client in collection.Clients)
                {
                    yield return client;
                }

                parameters["page"] = ++currentPage;
                collection = await ProcessRequest<ClientCollectionResponse>("clients", Method.GET, parameters);
            }
        }
    }
}
