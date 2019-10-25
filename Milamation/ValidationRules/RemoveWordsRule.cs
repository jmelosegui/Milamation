﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarvestClient.Model;

namespace Milamation.ValidationRules
{
    public class RemoveWordsRule : Rule
    {
        public override int Priority => 2;

        public List<string> Words => new List<string>()
        {
            { "PBI" },
            { "Bug" }
        };

        public override string Validate(TimesheetEntry timesheetEntry)
        {
            foreach (var word in Words)
            {
                if (timesheetEntry.Notes.StartsWith(word, StringComparison.OrdinalIgnoreCase))
                {
                    return $"Please remove the word {word}";
                }
            }            

            return null;
        }
    }
}