using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarvestClient.Model
{
    public class Project
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
