using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FlexCMS.Areas.Admin.Controllers
{
    /// <summary>
    /// Manage interaction with admin accounts
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// Sign out of the admin portion of the site
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Log on page
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOn()
        {
            return View();
        }

        /// <summary>
        /// Process log on
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LogOn(String userName, string password)
        {
            FormsAuthentication.SetAuthCookie("admin", true);
            return RedirectToAction("Index", "Home");

            //return View();
        }



    }
}
