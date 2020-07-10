using HarvestClient.Model;
using System.Linq;

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
                     && ProjectsRequiringPBI.Any(p => timesheetEntry.Project?.Name.StartsWith(p) == true)
                  )
                )
            {
                return "Please add PBI+Task";
            }

            return null;
        }
    }
}
