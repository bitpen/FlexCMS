using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlexCMS.BLL.Core
{

    /// <summary>
    /// Business Layer Models contracts for use when interacting with 
    /// an RoutesBO instance
    /// </summary>
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