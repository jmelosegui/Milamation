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

            return null;
        }
    }
}
