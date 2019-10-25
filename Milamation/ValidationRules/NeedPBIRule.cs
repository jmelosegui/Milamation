using System;
using System.Collections.Generic;
using HarvestClient.Model;

namespace Milamation.ValidationRules
{
    public class NeedPBIRule : Rule
    {
        public override int Priority => 1;

        //public List<string> TasksNotRequiringPBI => new List<string>()
        //{
        //    { "Admin" },
        //    { "Non-Billable Time" },
        //    { "Non-Billable Onboarding" },
        //    { "Onboarding" },
        //    { "Meetings" },
        //    { "Planning" },
        //    { "Project Management" }
        //};

        public List<string> TasksRequiringPBI => new List<string>()
        {
            { "Software Development" },
            { "QA" },
            { "UX Design" },
            { "Work Review" }
        };

        public List<string> ProjectsRequiringPBI => new List<string>()
        {
            { "Prism Apps" },
            { "Prism Platform" },
            { "GCDM" },
            { "Audit360" },
            { "Prism DevOps" },
            { "Prism Visualizations" },
            { "AWM Global Technology" },
        };

        public override string Validate(TimesheetEntry timesheetEntry)
        {
            if (timesheetEntry.Billable
                && !timesheetEntry.HasPBI
                && (
                     TasksRequiringPBI.Contains(timesheetEntry.Task?.Name)
                     && !ProjectsRequiringPBI.Contains(timesheetEntry.Project?.Name)
                  )
                )
            {
                return "Please add PBI+Task";
            }

            return null;
        }
    }
}
