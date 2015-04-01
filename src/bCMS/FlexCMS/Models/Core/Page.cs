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
    }
}