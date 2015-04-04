using FlexCMS.BLL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlexCMS.Controllers
{
    public class RouterController : Controller
    {
        /// <summary>
        /// Process a simple route (article)
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        public ActionResult SimpleRoute(string article)
        {
            return View();
        }

        /// <summary>
        /// Process a custom route
        /// </summary>
        /// <returns></returns>
        public ActionResult ComplexRoute()
        {
            var url = Request.RawUrl;
            url = url.Substring(1);
            var route = RoutesBO.Get(url);

            if (route != null)
            {
                return View();
            }



            return RouteNotFound(url);
        }


        #region Private methods
        /// <summary>
        /// Return result of Route not found for give path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private ActionResult RouteNotFound(String path)
        {
            Response.StatusCode = 404;
            Response.StatusDescription = "Unable to find route.";
            return Content("Unable to find route.");
        }


        #endregion Private Methods

    }
}
