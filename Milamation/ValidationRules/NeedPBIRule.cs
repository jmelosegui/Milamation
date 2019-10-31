using System;
using System.Collections.Generic;
using HarvestClient.Model;

namespace Milamation.ValidationRules
{
    public class NeedPBIRule : Rule
    {
        public override int Priority => 1;

        public override string Validate(TimesheetEntry timesheetEntry)
        {
            if (timesheetEntry.Billable
                && !timesheetEntry.HasPBI
                && (
                     TasksRequiringPBI.Contains(timesheetEntry.Task?.Name)
                     && ProjectsRequiringPBI.Contains(timesheetEntry.Project?.Name)
                  )
                )
            {
                return "Please add PBI+Task";
            }

            return null;
        }
    }
}
