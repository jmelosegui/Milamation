using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace HarvestClient.Model
{
    [DebuggerDisplay("{SpentDate}, {Notes}")]
    public class TimesheetEntry
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("spent_date")]
        public DateTime SpentDate { get; set; }

        [JsonProperty("hours")]
        public decimal Hours { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("billable")]
        public bool Billable { get; internal set; }
                
        [JsonProperty("project")]
        public Project Project { get; internal set; }

        public IdName User { get; set; }

        public IdName Task { get; set; }

        [NotMapped]
        public string PBI { get; set; }

        [NotMapped]
        public decimal RoundedHours { get; set; }

        [NotMapped]
        public string ReviewersComment { get; set; }

        [NotMapped]
        public string UserRoles { get; set; }

        [NotMapped]
        public bool HasPBI { get; set; }
    }
}
