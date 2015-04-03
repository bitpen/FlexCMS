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
    /// in relation to Custom pages
    /// </summary>
    public partial class PagesBO
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
        public PagesBO(UnitOfWork uow)
        {
            if (uow == null)
            {
                throw new ArgumentNullException("uow", "Valid UnitOfWork required.");
            }
            _uow = uow;
            _cmsContext = _uow.GetCmsContext();
        }

        /// <summary>
        /// Add a new page to the datastore
        /// </summary>
        /// <param name="page"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public Guid? Add(AddPageBLM page, out AddPageBLM.ValidationErrors errors)
        {
            Guid? id = null;

            errors = ValidatePage(page);

            if (errors.Any())
            {
                return null;
            }

            var model = new Page();
            model.Name = page.Name;
            model.Content = page.Content;
            model.DateCreated_utc = DateTime.UtcNow;
            model.CreatedBy = _cmsContext.ContextUserName;
            using (var transaction = new TransactionScope())
            {
                _cmsContext.Pages.Add(model);
                _cmsContext.SaveChanges();
                transaction.Complete();

                id = model.Id;
            }

            return id;
        }

        /// <summary>
        /// Find a page(s) within the datastore
        /// </summary>
        /// <returns></returns>
        public static List<PageSummaryBLM> Find()
        {
            var pages = new List<PageSummaryBLM>();

            List<Page> models = new List<Page>();
            using (var db = new CmsContext())
            {
                models = db.Pages.ToList();
            }

            pages = models.Select(i => new PageSummaryBLM()
            {
                Id = i.Id,
                Name = i.Name
            }).ToList();

            return pages;
        }

        /// <summary>
        /// Retrieve a page from the datastore
        /// </summary>
        /// <param name="id">Primary key of the page</param>
        /// <returns>Null if the page is not found</returns>
        public static PageBLM Get(Guid id)
        {
            Page model;
            using (var db = new CmsContext())
            {
                model = db.Pages.Find(id);

                if (model == null)
                {
                    return null;
                }
            }

            var page = new PageBLM();
            page.Id = model.Id;
            page.Name = model.Name;
            page.Content = model.Content;
            page.DateCreated_utc = model.DateCreated_utc;
            page.DateModified_utc = model.DateModified_utc;
            page.DatePublished_utc = model.DatePublished_utc;
            page.CreatedBy = model.CreatedBy;
            page.ModifiedBy = model.ModifiedBy;
            page.PublishedBy = model.PublishedBy;

            return page;
        }

        /// <summary>
        /// Update an existing page in the datastore
        /// </summary>
        /// <param name="page"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public Boolean Update(UpdatePageBLM page, out UpdatePageBLM.ValidationErrors errors)
        {
            errors = ValidatePage(page);

            if (errors.Any())
            {
                return false;
            }

            var model = _cmsContext.Pages.Find(page.Id);
            model.Name = page.Name;
            model.Content = page.Content;
            model.DateModified_utc = DateTime.UtcNow;
            model.ModifiedBy = _cmsContext.ContextUserName;
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
        /// Validate business and storage rules for a page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        private AddPageBLM.ValidationErrors ValidatePage(AddPageBLM page)
        {
            var errors = page.CreateValidationErrorsCollection();

            if (string.IsNullOrEmpty(page.Name))
            {
                errors.Add(AddPageBLM.ValidatableFields.Name, "Page name is required.");
            }
            else if(page.Name.Length > 50)
            {
                errors.Add(AddPageBLM.ValidatableFields.Name, "Maximum length of name is 50 characters.");
            }

            return errors;
        }

        #endregion Private Methods
    }
}