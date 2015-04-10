using bCommon.Validation;
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
        public enum RouteType
        {
            Section = 1,
            Page = 2,
            Article = 3
        }

       

        public class RouteSummaryBLM
        {
            public Guid Id { get; set; }
            public String Path { get; set; }
            public RouteType Type { get; set; }
        }
    }
}