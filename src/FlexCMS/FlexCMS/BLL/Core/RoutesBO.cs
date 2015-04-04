using FlexCMS.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

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
        /// Add a new custom route to the content system
        /// </summary>
        /// <param name="route">Unique path of the route</param>
        /// <param name="description">[Optional] Description of the route</param>
        /// <exception cref="ArgumentNullException">When the route is null or empty</exception>
        /// <returns>Null if route is duplicate of existing route</returns>
        public Guid? Add(String route, String description)
        {
            if (String.IsNullOrEmpty(route))
            {
                throw new ArgumentNullException("route", "Custom route cannot be empty.");
            }

            var cleanedRoute = CleanRoutePath(route);
            Guid? id;

            var model = new Route();
            model.Path = cleanedRoute;
            model.Description = description;

            if (_cmsContext.Routes.Any(i => i.Path.Equals(cleanedRoute)))
            {
                return null;
            }

            using (var transaction = new TransactionScope())
            {
                _cmsContext.Routes.Add(model);
                _cmsContext.SaveChanges();
                transaction.Complete();

                id = model.Id;
            }

            return id;
        }

        public static List<RouteBLM> Find()
        {
            var routes = new List<RouteBLM>();
            using (var db = new CmsContext())
            {
                using (var transaction = new TransactionScope())
                {
                    routes = db.Routes.ToList().Select(i => new RouteBLM()
                    {
                        Id = i.Id,
                        Path = i.Path,
                        Description = i.Description
                    }).ToList();

                    transaction.Complete();
                }
            }

            return routes;
        }

        /// <summary>
        /// Retrieve a route from the datastore
        /// </summary>
        /// <param name="id">Primary key of the route</param>
        /// <returns>Null if the route could not be found</returns>
        public static RouteBLM Get(Guid id)
        {
            Route data;
            var route = new RouteBLM();
            using (var db = new CmsContext())
            {
                using (var transaction = new TransactionScope())
                {
                    data = db.Routes.Find(id);
                }
            }

            if (data == null)
            {
                return null;
            }

            route.Id = data.Id;
            route.Path = data.Path;
            route.Description = data.Description;

            return route;
        }

        /// <summary>
        /// Retrieve a route from the datastore
        /// </summary>
        /// <param name="path">Full path of the route</param>
        /// <returns>Null if the route could not be found</returns>
        public static RouteBLM Get(string path)
        {
            Route data;
            var route = new RouteBLM();
            var cleanedPath = CleanRoutePath(path);
            using (var db = new CmsContext())
            {
                using (var transaction = new TransactionScope())
                {
                    data = db.Routes.FirstOrDefault(i => i.Path.Equals(cleanedPath));
                }
            }

            if (data == null)
            {
                return null;
            }

            route.Id = data.Id;
            route.Path = data.Path;
            route.Description = data.Description;

            return route;
        }

        /// <summary>
        /// Update an existing route in the system
        /// </summary>
        /// <param name="routeId">Primary key of the route</param>
        /// <param name="route">Unique path of the route</param>
        /// <param name="description">[Optional] Description of the route</param>
        /// <exception cref="ArgumentNullException">When the route id is Guid.Empty</exception>
        /// <exception cref="ArgumentNullException">When the route is empty</exception>
        /// <returns>True if successful</returns>
        public Boolean Update(Guid routeId, String route, string description)
        {
            if (routeId == Guid.Empty)
            {
                throw new ArgumentNullException("routeId", "Valid route id is required.");
            }

            if (string.IsNullOrEmpty(route))
            {
                throw new ArgumentNullException("route", "Custom route cannot be empty.");
            }

            var cleanedRoute = CleanRoutePath(route);
            if (_cmsContext.Routes.Any(i => i.Id != routeId && i.Path.Equals(cleanedRoute)))
            {
                return false;
            }

            var model = _cmsContext.Routes.Find(routeId);
            model.Path = cleanedRoute;
            model.Description = description;

            using (var transaction = new TransactionScope())
            {
                _cmsContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
                _cmsContext.SaveChanges();
                transaction.Complete();
            }

            return true;
        }

        #region Private Methods

        /// <summary>
        /// Clean and prepare the path of a route
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        private static String CleanRoutePath(string route)
        {
            var cleanedRoute = route.Trim().ToLower();
            while (cleanedRoute.StartsWith(@"/"))
            {
                cleanedRoute = cleanedRoute.Substring(1);
            }

            while (cleanedRoute.EndsWith(@"/"))
            {
                cleanedRoute = cleanedRoute.Substring(0, cleanedRoute.Length - 1);
            }

            return cleanedRoute;
        }

        #endregion Private Methods
    }
}