using bCommon.Validation;
using FlexCMS.Models.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
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
        public Guid? Add(AddSectionBLM section, 
            out ValidationErrors<SectionsBO.AddSectionBLM.ValidatableFields, String> errors)
        {
            Guid? id = null;

            errors = ValidateAddSection(section);

            if (errors.Any())
            {
                return null;
            }

            var parentPath = "";
            if (section.ParentSectionId != null)
            {
                var parent = _cmsContext.Sections.Find(section.ParentSectionId);
                if (parent != null)
                {
                    parentPath = parent.FullRoutePath;
                }
            }

            if (parentPath.Equals("/"))
            {
                //default root path
                parentPath = "";
            }

            var model = new Section();
            model.Name = section.Name;
            model.Description = section.Description;
            model.FullRoutePath = parentPath + "/" + section.Name;
            model.ParentSectionId = section.ParentSectionId;
            using (var transaction = new TransactionScope())
            {
                _cmsContext.Sections.Add(model);
                _cmsContext.SaveChanges();

                transaction.Complete();

                id = model.Id;
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
                Name = i.Name,
                Route = i.FullRoutePath
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

            SectionBLM section;

            var sql =
@"SELECT
TOP 1
Section.Id AS Id,
Section.Name AS Name,
Section.Description AS Description,
Section.FullRoutePath AS Route,
Section.ParentSectionId AS ParentSectionId
FROM
Section
WHERE Section.Id = @sectionId";
            var parameters = new object[1];
            parameters[0] = new SqlParameter("@sectionId", id);
            using (var db = new CmsContext())
            {
                section = db.Database.SqlQuery<SectionBLM>(sql, parameters).FirstOrDefault();
            }

            return section;
        
        }

        /// <summary>
        /// Retrieve a specific page of articles within a section
        /// </summary>
        /// <param name="id">Primary key of the section</param>
        /// <param name="page">Page to retrieve</param>
        /// <returns></returns>
        public static List<ArticlesBO.ArticleSummaryBLM> GetPagedArticles(Guid id, int page)
        {
            var articles = new List<ArticlesBO.ArticleSummaryBLM>();
            
            using (var db = new CmsContext())
            {
                var data = db.Articles
                            .Where(i => i.SectionId == id)
                            .OrderByDescending(i => i.DateCreated_utc)
                            .Skip(5 * (page - 1))
                            .Take(5);

                articles = data.Select(i => new ArticlesBO.ArticleSummaryBLM()
                {
                    Id = i.Id,
                    DatePublished_utc = i.DatePublished_utc,
                    Title = i.Title
                }).ToList();
            }

            return articles;
        }

        /// <summary>
        /// Update an existing section in the datastore
        /// </summary>
        /// <param name="section"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public Boolean Update(UpdateSectionBLM section, 
                            out ValidationErrors<SectionsBO.UpdateSectionBLM.ValidatableFields, String> errors)
        {
            errors = ValidateUpdateSection(section);

            if (errors.Any())
            {
                return false;
            }

            var parentPath = "";
            if (section.ParentSectionId != null)
            {
                var parent = _cmsContext.Sections.Find(section.ParentSectionId);
                if (parent != null)
                {
                    parentPath = parent.FullRoutePath;
                }
            }

            if (parentPath.Equals("/"))
            {
                //default root path
                parentPath = "";
            }

            var model = _cmsContext.Sections.Find(section.Id);
            model.Name = section.Name;
            model.Description = section.Description;
            model.FullRoutePath = parentPath + "/" + section.Name;
            model.ParentSectionId = section.ParentSectionId;
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
        private ValidationErrors<SectionsBO.AddSectionBLM.ValidatableFields, String>
                            ValidateSection(AddSectionBLM section)
        {
            var errors = section.CreateValidationErrorsCollection();

            if (section.Name == null)
            {
                section.Name = "";
            }
            if (string.IsNullOrEmpty(section.Name) && section.ParentSectionId != null)
            {
                errors.Add(AddSectionBLM.ValidatableFields.Name, "Only the default root section can have no name.");
            }
            else
            {
                if (section.Name.Length > 50)
                {
                    errors.Add(AddSectionBLM.ValidatableFields.Name, "Name length is limited to 50 characters");
                }

                if(Regex.IsMatch(section.Name, @"[^0-9A-Za-z\-]+")){
                    errors.Add(AddSectionBLM.ValidatableFields.Name, "Invalid characters in Name.");
                }
            }

            if (!String.IsNullOrEmpty(section.Description) && section.Description.Length > 500)
            {
                errors.Add(AddSectionBLM.ValidatableFields.Description, "Maximum length of Description is 500 characters.");
            }

            return errors;
        }

        /// <summary>
        /// Validate business and storage rules when creating a section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public ValidationErrors<AddSectionBLM.ValidatableFields, String>
            ValidateAddSection(AddSectionBLM section)
        {
            var errors = ValidateSection(section);


                if (_cmsContext.Sections.Any(i => i.ParentSectionId == section.ParentSectionId
                            && i.Name.Equals(section.Name)))
                {
                    errors.Add(UpdateSectionBLM.ValidatableFields.Name, "Section names must be unique");
                }
            

            return errors;
        }

        /// <summary>
        /// Validate business and storage rules when updating a section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public ValidationErrors<UpdateSectionBLM.ValidatableFields, String>
            ValidateUpdateSection(UpdateSectionBLM section)
        {
            var errors = ValidateSection(section);

            if (!String.IsNullOrEmpty(section.Name))
            {
                if (_cmsContext.Sections.Any(i => i.Id != section.Id
                            && i.ParentSectionId == section.ParentSectionId
                            && i.Name.Equals(section.Name)))
                {
                    errors.Add(UpdateSectionBLM.ValidatableFields.Name, "Section names must be unique");
                }
            }

            return errors;
        }

        #endregion Private Methods
    }
}