using Newtonsoft.Json;
using System;

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

        //public int NextPage
        //{
        //    get
        //    {
        //        int result = 0;

        //        if (NextPageUrl != null)
        //        {                    
        //            var uri = new Uri(NextPageUrl);
        //            var queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

        //            int.TryParse(queryDictionary["page"], out result);
        //        }

        //        return result;
        //    }
        //}
    }
}
