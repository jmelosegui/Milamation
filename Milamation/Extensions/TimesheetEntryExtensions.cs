using HarvestClient.Model;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Milamation.Extensions
{
    public static class TimesheetEntryExtensions
    {
        public static  void ApplyValidationRules(this TimesheetEntry timesheetEntry, IEnumerable<Milamation.ValidationRules.Rule> rules)
        {
            foreach (var rule in rules)
            {
                string validationError = rule.Validate(timesheetEntry);
                if (!string.IsNullOrEmpty(validationError))
                {
                    timesheetEntry.ReviewersComment = validationError;

                    break;
                }                
            }            
        }

        public static void RoundHours(this TimesheetEntry timesheetEntry)
        {
            var hours = Math.Truncate(timesheetEntry.Hours);
            var decimalPart = timesheetEntry.Hours - hours;

            switch (decimalPart)
            {
                case var _ when (decimalPart > 0M && decimalPart < .25M):
                    timesheetEntry.RoundedHours = hours + .25M;
                    break;
                case var _ when (decimalPart > .25M && decimalPart < .5M):
                    timesheetEntry.RoundedHours = hours + .5M;
                    break;
                case var _ when decimalPart > .5M && decimalPart < .75M:
                    timesheetEntry.RoundedHours = hours + .75M;
                    break;
                case var _ when (decimalPart > .75M):
                    timesheetEntry.RoundedHours = hours + 1M;
                    break;
                default:
                    timesheetEntry.RoundedHours = timesheetEntry.Hours;
                    break;
            }
        }

        public static void CompletePBIColumn(this TimesheetEntry timesheetEntry)
        {
            if (timesheetEntry.Task?.Name?.IndexOf("planning", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                timesheetEntry.PBI = "Planning";
            }
            else if (timesheetEntry.Notes?.IndexOf("scrum", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                timesheetEntry.PBI = "Scrum";
            }
            else
            {
                if (!string.IsNullOrEmpty(timesheetEntry.Notes))
                {
                    string pattern = @"^\s*(?<PBI>\d+)\.*";
                    var matches = Regex.Matches(timesheetEntry.Notes, pattern);
                    if (matches.Count > 0)
                    {
                        timesheetEntry.PBI = matches[0].Groups["PBI"].Value;
                    }
                }                
            }
             
        }
    }
}
