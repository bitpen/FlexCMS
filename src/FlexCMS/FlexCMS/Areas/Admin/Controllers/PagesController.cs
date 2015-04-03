﻿using FlexCMS.BLL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlexCMS.BLL;

namespace FlexCMS.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        public ActionResult Add()
        {
            var view = new AddPage();
            return View(view);
        }

        [HttpPost]
        public ActionResult Add(AddPage page)
        {
            var blm = new PagesBO.AddPageBLM();
            blm.Name = page.Name;
            blm.Content = page.Content;
            PagesBO.AddPageBLM.ValidationErrors errors;
            Guid? id;
            using (var uow = new UnitOfWork("jt"))
            {
                var bo = new PagesBO(uow);
                id = bo.Add(blm, out errors);
            }

            if (id != null)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

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

        public ActionResult Edit(Guid id)
        {
            var page = PagesBO.Get(id);

            var view = new EditPage();
            view.PageId = page.Id;
            view.Name = page.Name;
            view.Content = page.Content;
            return View(view);
        }

        [HttpPost]
        public ActionResult Edit(EditPage page)
        {
            var blm = new PagesBO.UpdatePageBLM();
            blm.Id = page.PageId;
            blm.Name = page.Name;
            blm.Content = page.Content;
            PagesBO.UpdatePageBLM.ValidationErrors errors;
            Boolean success;
            using (var uow = new UnitOfWork("jt"))
            {
                var bo = new PagesBO(uow);
                success = bo.Update(blm, out errors);
            }

            if (success)
            {
                return RedirectToAction("Index");
            }

            return View();
        }


        #region View Models

        public class AddPage
        {
            public string Name { get; set; }
            public String Content { get; set; }
        }

        public class EditPage : AddPage
        {
            public Guid PageId { get; set; }
        }

        public class PageListing
        {
            public Guid PageId { get; set; }
            public String Name { get; set; }
        }

        #endregion View Models
    }
}