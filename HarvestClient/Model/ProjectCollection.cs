using Newtonsoft.Json;
using System.Collections.Generic;

namespace HarvestClient.Model
{
    public class ProjectCollectionResponse : CollectionResponse
    {
        public ProjectCollectionResponse()
        {
            Projects = new List<Project>();
        }

        [JsonProperty("projects")]
        public IEnumerable<Project> Projects { get; private set; }
    }
}
