﻿using System;
using System.Collections.Generic;
using HarvestClient.Model;

namespace Milamation.ValidationRules
{
    public class NeedProperTaskRule : Rule
    {
        public override int Priority => 1;

        public List<string> TasksNotRequiringPBI => new List<string>()
        {
            { "Admin" },
            { "Non-Billable Time" },
            { "Non-Billable Onboarding" },
            { "Onboarding" },
            { "Meetings" },
            { "Planning" },
            { "Project Management" }
        };

        public override string Validate(TimesheetEntry timesheetEntry)
        {
            if (timesheetEntry.Billable
                && timesheetEntry.HasPBI == true
                && TasksNotRequiringPBI.Contains(timesheetEntry.Task?.Name))
            {
                return "Please update the task name (i.e. Software Development)";
            }

            return null;
        }
    }
}
