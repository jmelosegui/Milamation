using System;
using System.Collections.Generic;
using System.Text;
using HarvestClient.Model;

namespace Milamation.ValidationRules
{
    public abstract class Rule
    {
        public abstract int Priority { get; }

        public abstract string Validate(TimesheetEntry timesheetEntry);
        
    }
}
