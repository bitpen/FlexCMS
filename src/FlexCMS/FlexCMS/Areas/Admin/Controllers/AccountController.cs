using bCommon.Security;
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
        /// Update the password for the current account
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Process a change password request for the current account
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(string oldPassword, string newPassword)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LogOn");
            }

            var username = User.Identity.Name;
            var auth = new SQLUserPassAuth();
            if (auth.ChangePassword(username, null, oldPassword, newPassword))
            {
                return RedirectToAction("Settings");
            }
           
            return View();
        }

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
            var auth = new SQLUserPassAuth();
            if (auth.Authenticate(userName, null, password))
            {
                FormsAuthentication.SetAuthCookie("admin", true);
                return RedirectToAction("Index", "Home");
            }
            
            return View();
        }

        /// <summary>
        /// Main landing page for individual account settings
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Settings()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LogOn");
            }

            return View();
        }
    }
}
