using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace HarvestClient.Model
{
    [DebuggerDisplay("{Id} - {Name}")]
    public class Client
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
