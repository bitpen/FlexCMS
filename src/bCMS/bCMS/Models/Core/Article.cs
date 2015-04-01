using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace bCMS.Models.Core
{
    /// <summary>
    /// Storage model binding for an Article
    /// </summary>
    [Table("Article")]
    public class Article
    {
        /// <summary>
        /// Primary storage key
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Main title of the article
        /// </summary>
        [Required(ErrorMessage="Title is required for all Articles")]
        [MaxLength(250, ErrorMessage="The maximum length of the Title is 250 characters")]
        public String Title { get; set; }

        /// <summary>
        /// Unique reference name for building permalinks/searching/indexing
        /// </summary>
        [Required(ErrorMessage="Alias is required for all Articles")]
        [MaxLength(250, ErrorMessage="The maxiumum length of the Alias is 250 characters")]
        public String Alias{ get; set;}


        /// <summary>
        /// Main content of the article
        /// </summary>
        [MaxLength]
        public String Content { get; set; }

        /// <summary>
        /// Timestamp the article was first commited to storage
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated_utc { get; set;}

        /// <summary>
        /// Party responsible for the creation of the article
        /// </summary>
        [Required(ErrorMessage="CreatedBy is required for all Articles")]
        [MaxLength(50, ErrorMessage="The maxiumum length of CreatedBy is 50 characters")]
        public String CreatedBy { get; set;}

        /// <summary>
        /// Timestamp the article was last modified
        /// </summary>
        public DateTime? DateModified_utc{ get; set;}

        /// <summary>
        /// Party responsible for the last modification of the article
        /// </summary>
        [MaxLength(50, ErrorMessage="The maximum length of ModifiedBy is 50 characters")]
        public String ModifiedBy { get; set;}

        /// <summary>
        /// Timestamp the article was published
        /// </summary>
        public DateTime? DatePublished_utc{ get; set;}

        /// <summary>
        /// Party responsible for the publication of the article
        /// </summary>
        [MaxLength(50, ErrorMessage="The maximum length of PublishedBy is 50 characters")]
        public String PublishedBy { get; set;}
    }
}