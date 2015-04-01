using FlexCMS.Models.Core;
using bCommon.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlexCMS.BLL.Core
{
    /// <summary>
    /// Business Layer Models contracts for use when interacting with 
    /// an ArticlesBO instance
    /// </summary>
    public partial class ArticlesBO
    {
        /// <summary>
        /// Contract requirements for adding a new article to the datastore
        /// </summary>
        public class AddArticleBLM : AbstractValidatableBLM<AddArticleBLM.ValidatableFields, String>
        {
            public AddArticleBLM()
            {
                Tags = new List<String>();
            }

            public String Title { get; set; }
            public String Alias { get; set; }
            public String Content { get; set; }
            public List<String> Tags { get; set; }

            /// <summary>
            /// Fields that the business layer will validate against
            /// </summary>
            public new enum ValidatableFields
            {
                General = 1,
                Title = 2,
                Alias = 3,
            }
        }

        /// <summary>
        /// Contract requirements for updating an existing article in the datastore
        /// </summary>
        public class UpdateArticleBLM : AddArticleBLM
        {
            public Guid Id { get; set; }
        }

        /// <summary>
        /// Contract for retrieving an article from the datastore
        /// </summary>
        public class ArticleBLM : UpdateArticleBLM
        {
            
        }

        /// <summary>
        /// Contract for retrieving a summary of an article from the datastore
        /// </summary>
        public class ArticleSummaryBLM
        {
            public Guid Id { get; set; }
            public String Title { get; set; }
            public DateTime? DatePublished_utc { get; set; }
        }
    }
}