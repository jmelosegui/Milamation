using Newtonsoft.Json;
using System.Collections.Generic;

namespace HarvestClient.Model
{
    public class TimeEntryCollection : CollectionResponse
    {
        public TimeEntryCollection()
        {
            TimeEntries = new List<TimesheetEntry>();
        }

        [JsonProperty("time_entries")]
        public IEnumerable<TimesheetEntry> TimeEntries { get; private set; }
    }
}
