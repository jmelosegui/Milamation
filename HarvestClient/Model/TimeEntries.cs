﻿using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HarvestClient.Model
{
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

        public IdName Project { get; internal set; }

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
    }
}
