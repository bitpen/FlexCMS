
using FlexCMS.BLL.Core;
using FlexCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlexCMS.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            
            //var tagBO = new TagsBO(new BLL.UnitOfWork("jt"));
            //tagBO.Add("Tag3");

            //var add = new ArticlesBO.AddArticleBLM();
            //add.Title = "First";
            //add.Alias = Guid.NewGuid().ToString();
            //add.Content = "SDFSDFKJSDKF";
            //add.Tags.Add("tag1 ");
            //add.Tags.Add("tag2 ");

            //var bo = new ArticlesBO(new BLL.UnitOfWork("jt"));
            //ArticlesBO.AddArticleBLM.ValidationErrors errors;
            //var id = bo.Add(add, out errors);
            

            return View();
        }


        public ActionResult Route(string route)
        {
            return RedirectToAction("Index");

        }

    }
}
