using bCommon.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlexCMS.BLL.Core
{
    /// <summary>
    /// Business Layer Models contracts for use when interacting with 
    /// an PagesBO instance
    /// </summary>
    public partial class PagesBO
    {
        public class AddPageBLM : AbstractValidatableBLM<AddPageBLM.ValidatableFields, String>
        {
            public String Name { get; set; }
            public String Content { get; set; }
            public Guid RouteId { get; set; }

            public enum ValidatableFields
            {
                General = 1,
                Name = 2,
                RouteId = 3
            }
        }
    }
}