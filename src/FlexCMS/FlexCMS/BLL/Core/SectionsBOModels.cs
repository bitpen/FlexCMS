using bCommon.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlexCMS.BLL.Core
{

    /// <summary>
    /// Business Layer Models contracts for use when interacting with 
    /// a SectionsBO instance
    /// </summary>
    public partial class SectionsBO
    {
        /// <summary>
        /// Contract requirements for adding a new article to the datastore
        /// </summary>
        public class AddSectionBLM : AbstractValidatableBLM<AddSectionBLM.ValidatableFields, String>
        {

            public String Name { get; set; }
            public String Description { get; set; }

            /// <summary>
            /// Fields that the business layer will validate against
            /// </summary>
            public new enum ValidatableFields
            {
                General = 1,
                Name = 2,
                Description = 3,
            }
        }

        /// <summary>
        /// Contract requirements for updating an existing article in the datastore
        /// </summary>
        public class UpdateSectionBLM : AddSectionBLM
        {
            public Guid Id { get; set; }
        }

        /// <summary>
        /// Contract for retrieving an article from the datastore
        /// </summary>
        public class SectionBLM : UpdateSectionBLM
        {

        }

        /// <summary>
        /// Contract for retrieving a summary of an article from the datastore
        /// </summary>
        public class SectionSummaryBLM
        {
            public Guid Id { get; set; }
            public String Name { get; set; }
        }
    }
}