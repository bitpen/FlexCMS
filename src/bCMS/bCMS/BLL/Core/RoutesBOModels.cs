using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bCMS.BLL.Core
{
    public partial class RoutesBO
    {
        public class RouteBLM
        {
            public Guid Id { get; set; }
            public string Path { get; set; }
            public string Description { get; set; }
        }
    }
}