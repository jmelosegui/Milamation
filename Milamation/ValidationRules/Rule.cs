using HarvestClient.Model;
using System.Collections.Generic;

namespace Milamation.ValidationRules
{
    public abstract class Rule
    {
        public abstract int Priority { get; }

        public abstract string Validate(TimesheetEntry timesheetEntry);

        public virtual bool IsEnabled => true;

        public virtual List<string> ProjectsRequiringPBI => new List<string>()
        {
            { "Prism Apps" },
            { "Prism Platform" },
            { "GCDM" },
            { "Audit360" },
            { "Prism DevOps" },
            { "Prism Visualizations" },
            { "AWM Global Technology" },
        };

        public virtual List<string> TasksRequiringPBI => new List<string>()
        {
            { "Software Development" },
            { "QA" },
            { "UX Design" },
            { "Work Review" },
            { "Project Management" }
        };
    }
}
