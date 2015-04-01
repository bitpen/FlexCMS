using FlexCMS.BLL;
using FlexCMS.BLL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlexCMS.Areas.Admin.Controllers
{
    public class RoutesController : Controller
    {
        //Get: View for adding a new route
        public ActionResult Add()
        {
            var route = new NewRoute();
            return View(route);
        }

        //POST: Processing adding of a new route
        [HttpPost]
        public ActionResult Add(NewRoute route)
        {
            using (var uow = new UnitOfWork(HttpContext.User.Identity.Name))
            {
                var bo = new RoutesBO(uow);
                var id = bo.Add(route.Path, route.Description);
                if (id != null)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(route);
        }

        // GET: Edit view for updating a route
        public ActionResult Edit(Guid id)
        {
            var data = RoutesBO.Get(id);
            var route = new EditRoute();
            route.RouteId = data.Id;
            route.Path = data.Path;
            route.Description = data.Description;

            return View(route);
        }

        //POST: Process update of a route
        [HttpPost]
        public ActionResult Edit(EditRoute route)
        {
            using (var uow = new UnitOfWork(HttpContext.User.Identity.Name))
            {
                var bo = new RoutesBO(uow);
                if (bo.Update(route.RouteId, route.Path, route.Description))
                {
                    return RedirectToAction("Index");
                }
            }


            return View(route);
        }

        //
        // GET: Landing page of routes management
        public ActionResult Index()
        {
            var data = RoutesBO.Find();
            var routes = new List<RouteListing>();
            foreach (var item in data)
            {
                var route = new RouteListing();
                route.RouteId = item.Id;
                route.Path = item.Path;
                route.Description = item.Description;
                routes.Add(route);
            }

            return View(routes);
        }

        #region View Models

        public class EditRoute : NewRoute
        {
            public Guid RouteId { get; set; }
        }

        /// <summary>
        /// New route view model
        /// </summary>
        public class NewRoute
        {
            public String Path { get; set; }
            public String Description { get; set; }
        }

        /// <summary>
        /// Listing of a route
        /// </summary>
        public class RouteListing
        {
            public Guid RouteId { get; set; }
            public String Path { get; set; }
            public string Description { get; set; }
        }

        #endregion View Models
    }
}
