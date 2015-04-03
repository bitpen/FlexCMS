using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FlexCMS.Models.Core
{
    /// <summary>
    /// Storage model binding for a Page
    /// </summary>
    [Table("Page")]
    public class Page
    {
        /// <summary>
        /// Primary storage key
        /// </summary>
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Common name of the page
        /// </summary>
        [MaxLength(50, ErrorMessage = "The maximum length of Name is 50 characters")]
        [Required(ErrorMessage = "Name is required for all pages")]
        public String Name { get; set;}
        

        /// <summary>
        /// Main content of the page
        /// </summary>
        [MaxLength]
        public String Content { get; set;}

        /// <summary>
        /// Timestamp the page was originally created
        /// </summary>
        [Required(ErrorMessage = "Creation Date is required."]
        public DateTime DateCreated_utc { get; set; }


        /// <summary>
        /// Party responsible for the creation of the page
        /// </summary>
        [MaxLength(50, ErrorMessage = "The maximum lenth of created by is 50 characters.")]
        [Required(ErrorMessage = "CreatedBy is required"]
        public string CreatedBy { get; set; }


        /// <summary>
        /// Timestamp the page was last modified
        /// </summary>
        public DateTime? DateModified_utc { get; set;}


        /// <summary>
        /// Party responsible for the last edit
        /// </summary>
        [MaxLength(50, ErrorMessage = "The maximum lenth of ModifiedBy is 50 characters.")]
        [Required(ErrorMessage = "ModifiedBy is required"]
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Timestamp the page was published
        /// </summary>
        public DateTime? DatePublished_utc { get; set;}


        /// <summary>
        /// Party responsible for the published page
        /// </summary>
        [MaxLength(50, ErrorMessage = "The maximum lenth of PublishedBy is 50 characters.")]
        [Required(ErrorMessage = "PublishedBy is required"]
        public string PublishedBy { get; set; }
    }
}