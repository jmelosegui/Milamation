using HarvestClient.Model;

namespace Milamation.ValidationRules
{
    public abstract class Rule
    {
        public abstract int Priority { get; }

        public abstract string Validate(TimesheetEntry timesheetEntry);

        public virtual bool IsEnabled => true;
        
    }
}
