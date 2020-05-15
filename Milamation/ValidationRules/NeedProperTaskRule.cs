using HarvestClient.Model;

namespace Milamation.ValidationRules
{
    public class NeedProperTaskRule : Rule
    {
        public override int Priority => 2;

        public override string Validate(TimesheetEntry timesheetEntry)
        {
            if (!TasksRequiringPBI.Contains(timesheetEntry.Task.Name)
               && timesheetEntry.HasPBI)
            {
                return "Please update the task name (i.e. Software Development)";
            }

            return null;
        }
    }
}
