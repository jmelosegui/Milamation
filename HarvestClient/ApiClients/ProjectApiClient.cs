using HarvestClient.Model;
using HarvestClient.Utils;
using RestSharp;
using System.Collections.Generic;

namespace HarvestClient.ApiClients
{
    public class ProjectApiClient : BaseApiClient
    {
        public ProjectApiClient(string bearerToken, int accountId) 
            : base(bearerToken, accountId)
        {
        }

        public async IAsyncEnumerable<Project> List(int? clientId = null, bool? isActive = true)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            int currentPage = 1;
            FluentDictionary.For(parameters)
                            .Add("client_id", clientId, () => clientId.HasValue)
                            .Add("is_active", isActive.HasValue ? isActive.ToString().ToLower() : null, () => isActive.HasValue)
                            .Add("page", currentPage);

            var collection = await ProcessRequest<ProjectCollectionResponse>("projects", Method.GET, parameters);
            
            while (collection.TotalPages >= currentPage)
            {
                foreach (var project in collection.Projects)
                {
                    yield return project;
                }
                
                parameters["page"] = ++currentPage;
                collection = await ProcessRequest<ProjectCollectionResponse>("projects", Method.GET, parameters);
            }
        }        
    }
}
