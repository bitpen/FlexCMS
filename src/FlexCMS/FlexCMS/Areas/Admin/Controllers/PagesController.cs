using FlexCMS.BLL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlexCMS.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        //
        // GET: /Admin/Pages/
        public ActionResult Index()
        {
            var data = PagesBO.Find();
            var pages = data.Select(i => new PageListing()
            {
                PageId = i.Id,
                Name = i.Name
            }).ToList();

            return View(pages);
        }


        #region View Models

        public class PageListing
        {
            public Guid PageId { get; set; }
            public String Name { get; set; }
        }

        #endregion View Models
    }
}
