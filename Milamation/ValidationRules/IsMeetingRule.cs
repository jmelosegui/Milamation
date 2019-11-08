using System;
using HarvestClient.Model;

namespace Milamation.ValidationRules
{
    public class IsMeetingRule : Rule
    {
        public override int Priority => 1;

        public override string Validate(TimesheetEntry timesheetEntry)
        {
            if (timesheetEntry.Billable
                && timesheetEntry.PBI?.Contains("Scrum") == true
                && timesheetEntry.Task?.Name?.IndexOf("meeting", StringComparison.InvariantCultureIgnoreCase) < 0
                && timesheetEntry.Task?.Name?.IndexOf("Project Management", StringComparison.InvariantCultureIgnoreCase) < 0)
            {
                return "Please update the task to Meetings";
            }

            if(timesheetEntry.Task?.Name?.IndexOf("meeting", StringComparison.InvariantCultureIgnoreCase) >= 0
                && (
                    timesheetEntry.Notes?.IndexOf("Standup", StringComparison.InvariantCultureIgnoreCase) >= 0 
                    || timesheetEntry.Notes?.IndexOf("Stand-up", StringComparison.InvariantCultureIgnoreCase) >= 0
                    || timesheetEntry.Notes?.IndexOf("Stand up", StringComparison.InvariantCultureIgnoreCase) >= 0
                  ))
            {
                return "Please replace the word Standup to Scrum";
            }

            if (timesheetEntry.Task?.Name?.IndexOf("meeting", StringComparison.InvariantCultureIgnoreCase) >= 0
                && timesheetEntry.PBI?.Contains("Scrum") != true)
            {
                return "Please add the word Scrum";
            }

            return null;
        }
    }
}
