using Newtonsoft.Json;
using System.Collections.Generic;

namespace HarvestClient.Model
{
    public class TimeEntriesCollection : CollectionResponse
    {
        public TimeEntriesCollection()
        {
            TimeEntries = new List<TimesheetEntry>();
        }

        [JsonProperty("time_entries")]
        public IEnumerable<TimesheetEntry> TimeEntries { get; private set; }
    }
}
