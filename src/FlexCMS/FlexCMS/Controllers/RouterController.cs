using FlexCMS.Areas.Admin.Controllers;
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
        /// Process a custom route
        /// </summary>
        /// <returns></returns>
        public ActionResult ComplexRoute(string path)
        {
            
            //var url = Request.Path;
            var url = path ?? "/";
            int page;
            url = ParseRoute(url, out page);
            var route = RoutesBO.Check(url);

            if (route != null)
            {
                return GenerateRouteSpecificView(route);
            }

            return RouteNotFound(url);
        }


        #region Private methods
        
        /// <summary>
        /// Determine and generate the proper view for the given route
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        private ActionResult GenerateRouteSpecificView(RoutesBO.RouteSummaryBLM route)
        {
            if (route.Type == RoutesBO.RouteType.Section)
            {
                return GenerateSiteSectionView(route);
            }

            return View();
        }

        /// <summary>
        /// Generate the specific page requested for the site section requested 
        /// in the route
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        private ActionResult GenerateSiteSectionView(RoutesBO.RouteSummaryBLM route)
        {
            var data = SectionsBO.GetPagedArticles(route.Id, 1);
            var articles = data.Select(i => new ArticlesController.ArticleListing()
            {
                ArticleId = i.Id,
                DatePublished_utc = i.DatePublished_utc,
                Title = i.Title
            }).ToList();
            return View("SectionPage",articles);
        }

        /// <summary>
        /// Parse the details of a route
        /// </summary>
        /// <param name="path"></param>
        /// <param name="page">if pagination is requested, out put the page</param>
        /// <returns></returns>
        private String ParseRoute(string path, out int page)
        {
            page = 1;
            var cleanedRoute = path;
            
            if(cleanedRoute.StartsWith("/")){
                cleanedRoute = cleanedRoute.Substring(1); //strip off leading '/'
            }
            
            if (cleanedRoute.EndsWith("/"))
            {
                cleanedRoute = cleanedRoute.Substring(0, cleanedRoute.Length - 1);
            }

            var splitRoute = (cleanedRoute.Split('/') ?? new string[0]).ToList();
            var splitCount = splitRoute.Count;

            if (splitCount == 1) //simple route
            {
                return "/" + cleanedRoute;
            }

            var lastSeg = splitRoute[splitCount - 1].ToLower();
            var nextToLastSeg = splitRoute[splitCount - 2].ToLower();

            if (nextToLastSeg.Equals("page")) //pagination requested
            {
                page = int.Parse(lastSeg); //get page #

                var actualRoute = "";
                for (var i = 0; i < splitRoute.Count - 2; i++)
                {
                    actualRoute += "/" + splitRoute[i];
                }
                return actualRoute;
            }
            else if (lastSeg.Equals("page"))
            {
                page = 1;
                var actualRoute = "";
                for (var i = 0; i < splitRoute.Count - 1; i++)
                {
                    actualRoute += "/" + splitRoute[i];
                }
                return actualRoute;
            }

            return "/" + cleanedRoute;
        }


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
