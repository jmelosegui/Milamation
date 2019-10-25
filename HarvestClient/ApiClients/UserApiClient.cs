using HarvestClient.Model;
using RestSharp;
using System.Threading.Tasks;

namespace HarvestClient.ApiClients
{
    public class UserApiClient : BaseApiClient
    {
        public UserApiClient(string bearerToken, string accountId) 
            : base(bearerToken, accountId)
        {
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var result = await ProcessRequest<User>($"users/{userId}", Method.GET);

            return result;
        }

        public async Task<User> GetCurrentAsync()
        {
            var result = await ProcessRequest<User>($"users/me", Method.GET);

            return result;
        }
    }
}
