using FlexCMS.Models.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FlexCMS.BLL
{
    public class CmsContext : DbContext
    {
        private string _contextUserName;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="userName">User name of the party responsible for the context.  Used for audit purposed</param>
        public CmsContext(string userName = "system")
            : base("FlexCMS")
        {
            _contextUserName = userName;
        }

        /// <summary>
        /// Retrieve the user name of the party responsible for all changes during the duration of
        /// the context
        /// </summary>
        public String ContextUserName
        {
            get
            {
                return _contextUserName;
            }
        }

        /// <summary>
        /// Articles storage repository
        /// </summary>
        public DbSet<Article> Articles { get; set; }


        /// <summary>
        /// Custom page definitions
        /// </summary>
        public DbSet<Page> Pages { get; set; }

        /// <summary>
        /// Custom routes for use by the content system
        /// </summary>
        public DbSet<Route> Routes { get; set; }

        /// <summary>
        /// Tag storage repository
        /// </summary>
        public DbSet<Tag> Tags { get; set; }

    }
    
}