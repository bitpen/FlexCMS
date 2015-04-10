using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FlexCMS.Models.Core
{
    [Table("Routes")]
    public class Routes
    {
        public Guid Id { get; set; }
        public String Route { get; set; }
        public int Type { get; set; }
    }
}