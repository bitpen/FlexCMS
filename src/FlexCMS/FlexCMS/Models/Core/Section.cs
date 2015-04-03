using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FlexCMS.Models.Core
{
    /// <summary>
    /// Storage model binding for a site Section
    /// </summary>
    [Table("Section")]
    public class Section
    {
        /// <summary>
        /// Primary storage key
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }



        /// <summary>
        /// Common name of the page
        /// </summary>
        [MaxLength(50, ErrorMessage = "The maximum length of Name is 50 characters")]
        [Required(ErrorMessage = "Name is required for all sections")]
        public String Name { get; set; }
        

        /// <summary>
        /// General description of the section
        /// </summary>
        [MaxLength(500, ErrorMessage = "The maximum length of the description is 500 characters")]
        public String Description { get; set; }

    }
}