using HarvestClient.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Milamation.ValidationRules
{
    public class NoDescriptionRule : Rule
    {
        public override int Priority => 2;

        public override string Validate(TimesheetEntry timesheetEntry)
        {
            string pattern = @"\.*[a-zA-Z]+\.*";
            var matches = Regex.Matches(timesheetEntry.Notes, pattern);
            if (matches.Count == 0)
            {
                return "add description of work done";
            }

            return null;
        }
    }
}
