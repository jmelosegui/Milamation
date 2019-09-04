using HarvestClient.Model;

namespace Milamation.ValidationRules
{
    public class EmptyNotesRule : Rule
    {
        public override int Priority => 2;

        public override string Validate(TimesheetEntry timesheetEntry)
        {
            if (string.IsNullOrEmpty(timesheetEntry.Notes.Trim()))
            {
                return "Please enter description of work done + add PBI+Task if applicable";
            }

            return null;
        }
    }
}
