using Newtonsoft.Json;

namespace HarvestClient.Model
{
    public class CollectionResponse
    {
        [JsonProperty("page")]
        public int PageNumber { get; set; }

        [JsonProperty("per_page")]
        public int PageSize { get; set; }

        [JsonProperty("total_entries")]
        public int TotalEntries { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("next_page")]
        public int? NextPage { get; set; }

    }
}
