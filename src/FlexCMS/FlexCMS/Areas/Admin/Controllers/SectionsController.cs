using FlexCMS.BLL;
using FlexCMS.BLL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlexCMS.Areas.Admin.Controllers
{
    public class SectionsController : Controller
    {
        public ActionResult Add()
        {
            var view = new AddSection();
            return View(view);
        }

        [HttpPost]
        public ActionResult Add(AddSection section)
        {
            var blm = new SectionsBO.AddSectionBLM();
            blm.Name = section.Name;
            blm.Description = section.Description;
            blm.Route = section.RoutePath;
            SectionsBO.AddSectionBLM.ValidationErrors errors;
            Guid? id;
            using (var uow = new UnitOfWork("jt"))
            {
                var bo = new SectionsBO(uow);
                id = bo.Add(blm, out errors);
            }

            if (id != null)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        //
        // GET: /Admin/Sections/
        public ActionResult Index()
        {
            var data = SectionsBO.Find();
            var sections = data.Select(i => new SectionListing()
            {
                SectionId = i.Id,
                Name = i.Name
            }).ToList();

            return View(sections);
        }

        public ActionResult Edit(Guid id)
        {
            var section = SectionsBO.Get(id);

            var view = new EditSection();
            view.SectionId = section.Id;
            view.Name = section.Name;
            view.Description = section.Description;
            view.RoutePath = section.Route;

            return View(view);
        }

        [HttpPost]
        public ActionResult Edit(EditSection section)
        {
            var blm = new SectionsBO.UpdateSectionBLM();
            blm.Id = section.SectionId;
            blm.Name = section.Name;
            blm.Description = section.Description;
            SectionsBO.UpdateSectionBLM.ValidationErrors errors;
            Boolean success;
            using (var uow = new UnitOfWork("jt"))
            {
                var bo = new SectionsBO(uow);
                success = bo.Update(blm, out errors);
            }

            if (success)
            {
                return RedirectToAction("Index");
            }

            return View();
        }


        #region View Models

        public class AddSection
        {
            public string Name { get; set; }
            public String Description { get; set; }
            public String RoutePath { get; set; }
        }

        public class EditSection : AddSection
        {
            public Guid SectionId { get; set; }
        }

        public class SectionListing
        {
            public Guid SectionId { get; set; }
            public String Name { get; set; }
        }

        #endregion View Models

    }
}
