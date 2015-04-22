using bCommon.Validation;
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

            var data = SectionsBO.Find();
            view.AvailableParentSections = data.Select(i => new SectionListing(){
                SectionId = i.Id,
                Name = i.Name,
                FullRoute = i.Route
            }).ToList();

            return View(view);
        }

        [HttpPost]
        public ActionResult Add(AddSection section)
        {
            var blm = new SectionsBO.AddSectionBLM();
            blm.Name = section.Name;
            blm.Description = section.Description;
            blm.ParentSectionId = section.ParentSectionId;
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

            var data = SectionsBO.Find();
            section.AvailableParentSections = data.Select(i => new SectionListing()
            {
                SectionId = i.Id,
                Name = i.Name,
                FullRoute = i.Route
            }).ToList();

            //map validation errors
            var mappedErrors = errors.Map(_addSectionValidationMapper);

            foreach (var error in mappedErrors.ToList())
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            return View(section);
        }

        //
        // GET: /Admin/Sections/
        public ActionResult Index()
        {
            var data = SectionsBO.Find();
            var sections = data.Select(i => new SectionListing()
            {
                SectionId = i.Id,
                Name = i.Name,
                FullRoute = i.Route
            }).ToList();

            return View(sections);
        }

        public ActionResult Edit(Guid id)
        {
            var section = SectionsBO.Get(id);

            var view = new EditSection();

            var data = SectionsBO.Find();
            view.AvailableParentSections = data.Where(i => i.Id != section.Id).Select(i => new SectionListing()
            {
                SectionId = i.Id,
                Name = i.Name,
                FullRoute = i.Route,
            }).ToList();

            
            view.SectionId = section.Id;
            view.Name = section.Name;
            view.Description = section.Description;
            view.ParentSectionId = section.ParentSectionId;

            return View(view);
        }

        [HttpPost]
        public ActionResult Edit(EditSection section)
        {
            var blm = new SectionsBO.UpdateSectionBLM();
            blm.Id = section.SectionId;
            blm.Name = section.Name;
            blm.Description = section.Description;
            blm.ParentSectionId = section.ParentSectionId;
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

            var data = SectionsBO.Find();
            section.AvailableParentSections = data.Where(i => i.Id != section.SectionId).Select(i => new SectionListing()
            {
                SectionId = i.Id,
                Name = i.Name,
                FullRoute = i.Route
            }).ToList();

            //map validation errors
            var mappedErrors = errors.Map(_addSectionValidationMapper);

            foreach (var error in mappedErrors.ToList())
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            return View(section);
        }


        #region View Models

        public class AddSection
        {
            public AddSection()
            {
                AvailableParentSections = new List<SectionListing>();
            }

            public string Name { get; set; }
            public String Description { get; set; }
            public Guid? ParentSectionId { get; set; }

            public List<SectionListing> AvailableParentSections { get; set; }
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
