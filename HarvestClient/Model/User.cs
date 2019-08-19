using System.Collections.Generic;

namespace HarvestClient.Model
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<string> Roles { get; set; }
    }
}
