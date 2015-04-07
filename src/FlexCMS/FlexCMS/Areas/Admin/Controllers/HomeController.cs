using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlexCMS.Areas.Admin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //
        // GET: /Admin/Home/

        public ActionResult Index()
        {
            return View();
        }

    }
}
