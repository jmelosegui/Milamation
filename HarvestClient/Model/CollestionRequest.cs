using System;
using System.Collections.Generic;
using System.Text;

namespace HarvestClient.Model
{
    public class CollectionRequest
    {
        public int PageNumber { get; internal set; }
        public int PageSize { get; internal set; }
    }
}
