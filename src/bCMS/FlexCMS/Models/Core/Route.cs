using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FlexCMS.Models.Core
{
    /// <summary>
    /// Storage model binding for a Route
    /// </summary>
    [Table("Route")]
    public class Route
    {
        /// <summary>
        /// Primary storage key
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// General description of the route
        /// </summary>
        [MaxLength(500, ErrorMessage = "The maximum length of the description is 500 characters")]
        public String Description { get; set; }


        /// <summary>
        /// URL route for the path
        /// </summary>
        [MaxLength(500, ErrorMessage = "The paths for routes is currently restricted to 500 characters")]
        [Required(ErrorMessage = "Path is required for a route")]
        public String Path { get; set; }
    }
}