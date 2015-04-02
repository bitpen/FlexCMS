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
    /// in relation to Articles
    /// </summary>
    public partial class ArticlesBO
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
        public ArticlesBO(UnitOfWork uow)
        {
            if (uow == null)
            {
                throw new ArgumentNullException("uow", "Valid UnitOfWork required.");
            }
            _uow = uow;
            _cmsContext = _uow.GetCmsContext();
        }

        /// <summary>
        /// Add a new article to the datastore
        /// </summary>
        /// <param name="add"></param>
        /// <param name="errors"></param>
        /// <returns>Primary key of the new article</returns>
        public Guid? Add(AddArticleBLM article, 
                        out AddArticleBLM.ValidationErrors errors)
        {
            if (article == null)
            {
                throw new ArgumentNullException("article", "Valid article instance is required.");
            }

            errors = article.CreateValidationErrorsCollection();

            Guid? id = null;
            try
            {
                errors = ValidateAddArticle(article);

                if (errors.Any())
                {
                    return null;
                }
                
                var model = new Article();
                model.Id = Guid.NewGuid();
                MapEditableBLMContentToModel(article, ref model);
                model.DateCreated_utc = DateTime.UtcNow;
                model.CreatedBy = _cmsContext.ContextUserName;

                using (var transaction = new TransactionScope())
                {
                    _cmsContext.Articles.Add(model);
                    _cmsContext.SaveChanges();

                    AssignTagsToArticle(model.Id, article.Tags);

                    id = model.Id;
                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                //TODO: Log error
                errors.Add(AddArticleBLM.ValidatableFields.General,
                    "Error commiting changes to the datastore:");
            }

            return id;
        }

        /// <summary>
        /// Find articles in the datastore
        /// </summary>
        public static List<ArticleSummaryBLM> Find()
        {
            var articles = new List<ArticleSummaryBLM>();

            using (var cmsContext = new CmsContext())
            {
                var data = cmsContext.Articles
                            .OrderByDescending(i => i.PublishedBy)
                            .Select(i => new {i.Id, i.Title, i.DatePublished_utc}).ToList();

                articles = data.Select(i => new ArticleSummaryBLM()
                {
                    Id = i.Id,
                    Title = i.Title,
                    DatePublished_utc = i.DatePublished_utc != null ? i.DatePublished_utc.Value : (DateTime?)null
                }).ToList();
            }

            return articles;
        }

        /// <summary>
        /// Retrieve an article from the datastore
        /// </summary>
        /// <param name="id">Primary key of the article</param>
        /// <returns></returns>
        public static ArticleBLM Get(Guid id)
        {
            Article model = null;
            ArticleBLM article = null;
            try
            {
                using (var cmsContext = new CmsContext())
                {
                    model = cmsContext.Articles.Find(id);
                }

                if (model == null)
                {
                    return null;
                }

                article = new ArticleBLM();
                article.Id = model.Id;
                article.Title = model.Title;
                article.Alias = model.Alias;
                article.Content = model.Content;
            }
            catch (Exception ex)
            {
                //TODO: Log error
                throw new Exception("Error reading from datastore");
            }

            return article;
        }

        /// <summary>
        /// Update an existing article in the datastore
        /// </summary>
        /// <param name="article"></param>
        /// <param name="errors"></param>
        public void Update(UpdateArticleBLM article,
                            out UpdateArticleBLM.ValidationErrors errors)
        {
            if (article == null)
            {
                throw new ArgumentNullException("article", "Valid article instance is required.");
            }

            errors = article.CreateValidationErrorsCollection();

            try
            {
                errors = ValidateUpdateArticle(article);

                if (errors.Any())
                {
                    return;
                }

                using (var transaction = new TransactionScope())
                {
                    var model = _cmsContext.Articles.Find(article.Id);
                    MapEditableBLMContentToModel(article, ref model);
                    model.DateModified_utc = DateTime.UtcNow;
                    model.ModifiedBy = _cmsContext.ContextUserName;
                    _cmsContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    _cmsContext.SaveChanges();

                    AssignTagsToArticle(model.Id, article.Tags);

                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                //TODO: Log error
                errors.Add(AddArticleBLM.ValidatableFields.General,
                    "Error commiting changes to the datastore");
            }
        }


        #region Private Validation Methods

        /// <summary>
        /// Assign a tag to an article.
        /// </summary>
        /// <param name="tagId">Primary key of the tag</param>
        /// <param name="articleId">Primary key of the article</param>
        private void AssignTagToArticle(Guid articleId, Guid tagId)
        {
            var sql = "SELECT COUNT(*) FROM ArticleToTag WHERE ArticleId = @articleId AND TagId = @tagId";
            var parameters = new object[2];
            parameters[0] = new SqlParameter("@articleId", articleId);
            parameters[1] = new SqlParameter("@tagId", tagId);
            var count = _cmsContext.Database.SqlQuery<int>(sql, parameters).First();
            if (count > 0)
            {
                return;
            }

            sql = "INSERT INTO ArticleToTag(ArticleId, TagId) VALUES(@articleId, @tagId)";
            var insertParams = new object[2];
            insertParams[0] = new SqlParameter("@articleId", articleId);
            insertParams[1] = new SqlParameter("@tagId", tagId);
            var rows = _cmsContext.Database.ExecuteSqlCommand(sql, insertParams);
            if (rows != 1)
            {
                throw new Exception("Unable to assign tag to article in datastore");
            }
        }

        /// <summary>
        /// Assign a list of tags to an article
        /// </summary>
        /// <param name="articleId">Primary key of the article</param>
        /// <param name="tags">List of tags to assign</param>
        private void AssignTagsToArticle(Guid articleId, List<String> tags)
        {
            var tagBO = new TagsBO(_uow);
            foreach (var tag in tags)
            {
                var tagId = tagBO.Add(tag);
                if (tagId != null)
                {
                    AssignTagToArticle(articleId, (Guid)tagId);
                }
                else
                {
                    throw new Exception("Unable to create tag in the datastore.");
                }
            }
        }


        /// <summary>
        /// Map common editable article fields from the BLM to the storage model
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        private void MapEditableBLMContentToModel(AddArticleBLM article, ref Article model)
        {
            if (model == null)
            {
                model = new Article();
            }
            model.Title = article.Title;
            model.Alias = article.Alias;
            model.Content = article.Content;
        }

        /// <summary>
        /// Validate properties of a new article
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        private AddArticleBLM.ValidationErrors ValidateAddArticle(AddArticleBLM article)
        {
            var errors = ValidateArticle(article);

            //Check for alias uniqueness
            if (!String.IsNullOrEmpty(article.Alias) && _cmsContext.Articles.Any(i => i.Alias.ToUpper().Equals(article.Alias.ToUpper())))
            {
                errors.Add(AddArticleBLM.ValidatableFields.Alias, "Unique alias is required.");
            }

            return errors;
        }


        /// <summary>
        /// Validate general article properties
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        private AddArticleBLM.ValidationErrors 
            ValidateArticle(AddArticleBLM article)
        {
            var errors = article.CreateValidationErrorsCollection();

            if (String.IsNullOrEmpty(article.Title))
            {
                errors.Add(AddArticleBLM.ValidatableFields.Title, "Title is required");
            }

            if (String.IsNullOrEmpty(article.Alias))
            {
                errors.Add(AddArticleBLM.ValidatableFields.Alias, "Alias is required");
            }

            return errors;
        }

        /// <summary>
        /// Validate properties for updates to an article
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        private UpdateArticleBLM.ValidationErrors ValidateUpdateArticle(UpdateArticleBLM article)
        {
            var errors = ValidateArticle((AddArticleBLM)article);

            if (!String.IsNullOrEmpty(article.Alias))
            {
                if(_cmsContext.Articles.Any(i => i.Id != article.Id && i.Alias.ToUpper().Equals(article.Alias.ToUpper()))){
                    errors.Add(AddArticleBLM.ValidatableFields.Alias, "Unique alias is required.");
                }
            }

            return errors;
        }

        #endregion

    }
}