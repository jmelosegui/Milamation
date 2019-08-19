using System.Collections.Generic;

namespace HarvestClient.Model
{
    public class ClientCollectionResponse : CollectionResponse
    {
        public ClientCollectionResponse()
        {
            Clients = new List<Client>();
        }
                
        public IEnumerable<Client> Clients { get; private set; }
    }
}
