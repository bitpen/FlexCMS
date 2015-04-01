using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bCMS.Controllers
{
    public class RouterController : Controller
    {
        public ActionResult SimpleRoute(string article)
        {
            return View();
        }

        public ActionResult ComplexRoute()
        {
            return View();
        }

    }
}
