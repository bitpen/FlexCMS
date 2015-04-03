using FlexCMS.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

namespace FlexCMS.BLL.Core
{
    /// <summary>
    /// Business Layer object for interacting with the datastore 
    /// in relation to Sections
    /// </summary
    public partial class SectionsBO
    {
         /// <summary>
        /// Single unit of work which by to perform all class level work.
        /// </summary>
        private readonly UnitOfWork _uow;
        
        /// <summary>
        /// Reference to CMS context inside of _uow for conveneice
        /// </summary>
        private readonly CmsContext _cmsContext;

        /// <summary>
        /// Constructor to pass in an active unit of work
        /// </summary>
        /// <param name="uow"></param>
        /// <exception cref="ArgumentNullException">When not passed a valid UnitOfWork</exception>
        public SectionsBO(UnitOfWork uow)
        {
            if (uow == null)
            {
                throw new ArgumentNullException("uow", "Valid UnitOfWork required.");
            }
            _uow = uow;
            _cmsContext = _uow.GetCmsContext();
        }

        /// <summary>
        /// Add a new section to the datastore
        /// </summary>
        /// <param name="section"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public Guid? Add(AddSectionBLM section, out AddSectionBLM.ValidationErrors errors)
        {
            Guid? id = null;

            errors = ValidateSection(section);

            if (errors.Any())
            {
                return null;
            }

            var model = new Section();
            model.Name = section.Name;
            model.Description = section.Description;
            try
            {
                using (var transaction = new TransactionScope())
                {
                    _cmsContext.Sections.Add(model);
                    _cmsContext.SaveChanges();
                    transaction.Complete();

                    id = model.Id;
                }
            }
            catch (Exception ex)
            {

            }
            

            return id;
        }

        /// <summary>
        /// Find a section(s) within the datastore
        /// </summary>
        /// <returns></returns>
        public static List<SectionSummaryBLM> Find()
        {
            var sections = new List<SectionSummaryBLM>();

            List<Section> models = new List<Section>();
            using (var db = new CmsContext())
            {
                models = db.Sections.ToList();
            }

            sections = models.Select(i => new SectionSummaryBLM()
            {
                Id = i.Id,
                Name = i.Name
            }).ToList();

            return sections;
        }

        /// <summary>
        /// Retrieve a section from the datastore
        /// </summary>
        /// <param name="id">Primary key of the section</param>
        /// <returns>Null if the page is not found</returns>
        public static SectionBLM Get(Guid id)
        {
            Section model;
            using (var db = new CmsContext())
            {
                model = db.Sections.Find(id);

                if (model == null)
                {
                    return null;
                }
            }

            var section = new SectionBLM();
            section.Id = model.Id;
            section.Name = model.Name;
            section.Description = model.Description;

            return section;
        }

        /// <summary>
        /// Update an existing section in the datastore
        /// </summary>
        /// <param name="section"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public Boolean Update(UpdateSectionBLM section, out UpdateSectionBLM.ValidationErrors errors)
        {
            errors = ValidateSection(section);

            if (errors.Any())
            {
                return false;
            }

            var model = _cmsContext.Sections.Find(section.Id);
            model.Name = section.Name;
            model.Description = section.Description;
            using (var transaction = new TransactionScope())
            {
                _cmsContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
                _cmsContext.SaveChanges();
                transaction.Complete();
            }

            return true;
        }


        #region Private Methods

        /// <summary>
        /// Validate business and storage rules for a section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        private AddSectionBLM.ValidationErrors ValidateSection(AddSectionBLM section)
        {
            var errors = section.CreateValidationErrorsCollection();

            if (string.IsNullOrEmpty(section.Name))
            {
                errors.Add(AddSectionBLM.ValidatableFields.Name, "Page name is required.");
            }
            else if (section.Name.Length > 50)
            {
                errors.Add(AddSectionBLM.ValidatableFields.Name, "Maximum length of name is 50 characters.");
            }

            return errors;
        }

        #endregion Private Methods
    }
}