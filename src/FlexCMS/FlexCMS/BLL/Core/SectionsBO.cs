using FlexCMS.Models.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            model.FullRoutePath = "/" + section.Name;
            using (var transaction = new TransactionScope())
            {
                _cmsContext.Sections.Add(model);
                _cmsContext.SaveChanges();

                //var routesBO = new RoutesBO(_uow);
                //var routeId = routesBO.Add(section.Route, null, RoutesBO.RouteType.Section);

                //if (routeId == null)
                //{
                //    errors.Add(AddSectionBLM.ValidatableFields.Route, "Unable to create new route.");
                //    return null;
                //}


                //var sql = @"INSERT INTO RouteToSection(RouteId, SectionId) Values(@routeId, @sectionId)";
                //var parameters = new object[2];
                //parameters[0] = new SqlParameter("@routeId", routeId);
                //parameters[1] = new SqlParameter("@sectionId", model.Id);
                //var rows = _cmsContext.Database.ExecuteSqlCommand(sql, parameters);

                //if (rows != 1)
                //{
                //    errors.Add(AddSectionBLM.ValidatableFields.Route, "Unable to map new route.");
                //    return null;
                //}

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
Section.FullRoutePath AS Route
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
            model.FullRoutePath = "/" + section.Name;
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
                errors.Add(AddSectionBLM.ValidatableFields.Name, "Section name is required.");
            }
            else if (section.Name.Length > 50)
            {
                errors.Add(AddSectionBLM.ValidatableFields.Name, "Maximum length of Name is 50 characters.");
            }

            return errors;
        }

        #endregion Private Methods
    }
}