﻿using bCommon.Validation;
using FlexCMS.BLL;
using FlexCMS.BLL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlexCMS.Areas.Admin.Controllers
{
    [Authorize]
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
            ValidationErrors<SectionsBO.AddSectionBLM.ValidatableFields, String> errors;
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

            return View(view);
        }

        [HttpPost]
        public ActionResult Edit(EditSection section)
        {
            var blm = new SectionsBO.UpdateSectionBLM();
            blm.Id = section.SectionId;
            blm.Name = section.Name;
            blm.Description = section.Description;
            ValidationErrors<SectionsBO.UpdateSectionBLM.ValidatableFields, String> errors;
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
        }

        public class EditSection : AddSection
        {
            public Guid SectionId { get; set; }
        }

        public class SectionListing
        {
            public Guid SectionId { get; set; }
            public String Name { get; set; }
            public String FullRoute { get; set; }
        }

        #endregion View Models

        #region Private Properties for Validation

        private static Dictionary<SectionsBO.AddSectionBLM.ValidatableFields, String>
            _addSectionValidationMapper = new Dictionary<SectionsBO.AddSectionBLM.ValidatableFields,string>();

        static SectionsController()
        {

            _addSectionValidationMapper.Add(SectionsBO.AddSectionBLM.ValidatableFields.General, string.Empty);
            _addSectionValidationMapper.Add(SectionsBO.AddSectionBLM.ValidatableFields.Name, "Name");
            _addSectionValidationMapper.Add(SectionsBO.AddSectionBLM.ValidatableFields.Description, "Description");
        }

        #endregion Private Properties

    }
}
