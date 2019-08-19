using Newtonsoft.Json;

namespace HarvestClient.Model
{
    public class IdName
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
