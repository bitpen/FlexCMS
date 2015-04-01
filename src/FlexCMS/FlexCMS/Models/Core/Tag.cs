using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FlexCMS.Models.Core
{
    /// <summary>
    /// Storage model binding for a label/tag
    /// </summary>
    [Table("Tag")]
    public class Tag
    {
        /// <summary>
        /// Primary key of the tag/label
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        [MaxLength(50, ErrorMessage = "The maximum length of a tag name is 50 characters")]
        [Required(ErrorMessage = "Name is required for all tags")]
        public String Name { get; set; }
    }
}