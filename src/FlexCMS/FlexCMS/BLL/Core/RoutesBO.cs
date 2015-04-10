using FlexCMS.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using bCommon.Validation;

namespace FlexCMS.BLL.Core
{
    public partial class RoutesBO
    {
        /// <summary>
        /// Single unit of work which by to perform all class level work.
        /// </summary>
        private readonly UnitOfWork _uow;
        
        /// <summary>
        /// Reference to CMS context inside of _uow for conveneice
        /// </summary>
        private readonly CmsContext _cmsContext;

        /// <summary>
        /// Constructor to pass in an active unit of work
        /// </summary>
        /// <param name="uow"></param>
        /// <exception cref="ArgumentNullException">When not passed a valid UnitOfWork</exception>
        public RoutesBO(UnitOfWork uow)
        {
            if (uow == null)
            {
                throw new ArgumentNullException("uow", "Valid UnitOfWork required.");
            }
            _uow = uow;
            _cmsContext = _uow.GetCmsContext();
        }


        /// <summary>
        /// Check the validity and type of a route path
        /// </summary>
        /// <param name="path">Full path of the route</param>
        /// <returns>Null if the route could not be found</returns>
        public static RouteSummaryBLM Check(string path)
        {
            RouteSummaryBLM route = null;
            using (var db = new CmsContext())
            {
                using (var transaction = new TransactionScope())
                {
                    var data = db.Routeses.FirstOrDefault(i => i.Route.Equals(path));
                    if (data != null)
                    {
                        route = new RouteSummaryBLM();
                        route.Id = data.Id;
                        route.Path = data.Route;
                        route.Type = (RouteType) data.Type;
                    }
                       

                    transaction.Complete();
                }
            }


            return route;
        }

       


       
       

   
    }
}