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
    public class ArticlesController : Controller
    {
        //GET: Articles Management landing page
        public ActionResult Index()
        {
            var articles = ArticlesBO.Find();
            var view = articles.Select(i => new ArticleListing()
            {
                ArticleId = i.Id,
                Title = i.Title,
                DatePublished_utc = i.DatePublished_utc
            }).ToList();
            return View(view);
        }

        //GET: New article staring page
        public ActionResult New()
        {
            var view = new NewArticle();
            var sections = SectionsBO.Find();
            view.AvailableSections = sections.Select(i => new SectionsController.SectionListing()
            {
                SectionId = i.Id,
                Name = i.Name,
                FullRoute = i.Route
            }).ToList();


            return View(view);


        }

        //POST: Add new article
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult New(NewArticle article)
        {
            
            var add = new ArticlesBO.AddArticleBLM();
            add.Title = article.Title;
            add.Alias = article.Permalink;
            add.Content = article.Content;
            add.SectionId = article.SectionId;
            if (article.PublishDate.HasValue)
            {
                add.PublishDate_utc = article.PublishDate.Value.ToUniversalTime();
            }
            

            using (var uow = new UnitOfWork("jt"))
            {
                var articleBO = new ArticlesBO(uow);
                ValidationErrors<ArticlesBO.AddArticleBLM.ValidatableFields, String> errors;
                var id = articleBO.Add(add, out errors);

                if (id != null && !errors.Any())
                {
                    return RedirectToAction("Index");
                }
                
            }

            var sections = SectionsBO.Find();
            article.AvailableSections = sections.Select(i => new SectionsController.SectionListing()
            {
                SectionId = i.Id,
                Name = i.Name,
                FullRoute = i.Route
            }).ToList();

            return View(article);
        }

        //GET: Edit article page
        public ActionResult Edit(Guid id)
        {
            var article = ArticlesBO.Get(id);
            var view = new EditArticle();
            view.ArticleId = article.Id;
            view.Title = article.Title;
            view.Permalink = article.Alias;
            view.Content = article.Content;
            view.SectionId = article.SectionId;
            if (article.PublishDate_utc.HasValue)
            {
                view.PublishDate = article.PublishDate_utc.Value.ToLocalTime();
            }
            
            var sections = SectionsBO.Find();
            view.AvailableSections = sections.Select(i => new SectionsController.SectionListing()
            {
                SectionId = i.Id,
                Name = i.Name,
                FullRoute = i.Route
            }).ToList();

            return View(view);
        }

        //POST: Update an article
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(EditArticle article)
        {
            
            var update = new ArticlesBO.UpdateArticleBLM();
            update.Id = article.ArticleId;
            update.Title = article.Title;
            update.Alias = article.Permalink;
            update.Content = article.Content;
            update.SectionId = article.SectionId;
            if (article.PublishDate.HasValue)
            {
                update.PublishDate_utc = article.PublishDate.Value.ToUniversalTime();
            }
            
            ValidationErrors<ArticlesBO.UpdateArticleBLM.ValidatableFields, String> errors;
            using (var uow = new UnitOfWork("jt"))
            {

                var articlesBO = new ArticlesBO(uow);
                articlesBO.Update(update, out errors);

                if (!errors.Any())
                {
                    return RedirectToAction("Index");
                }
            
            
            }

            var sections = SectionsBO.Find();
            article.AvailableSections = sections.Select(i => new SectionsController.SectionListing()
            {
                SectionId = i.Id,
                Name = i.Name,
                FullRoute = i.Route
            }).ToList();

            return View(article);
        }

        #region View Models

        /// <summary>
        /// Article summary listing view model
        /// </summary>
        public class ArticleListing
        {
            public Guid ArticleId { get; set; }
            public String Title { get; set; }
            public DateTime? DatePublished_utc { get; set; }
            public DateTime? DatePublished_local
            {
                get
                {
                    if (DatePublished_utc == null)
                    {
                        return null;
                    }

                    return DatePublished_utc.Value.ToLocalTime();
                }
            }
            public String DatePublished_local_display
            {
                get
                {
                    if (DatePublished_local == null)
                    {
                        return "";
                    }

                    return String.Format("{0:MM/dd/yyyy hh:mm tt}", DatePublished_local.Value);
                }
            }

        }

        /// <summary>
        /// View model to collect updates to an article
        /// </summary>
        public class EditArticle : NewArticle
        {
            public Guid ArticleId { get; set; }
        }

        /// <summary>
        /// View model to collect new article data
        /// </summary>
        public class NewArticle
        {
            public NewArticle()
            {
                AvailableSections = new List<SectionsController.SectionListing>();
            }

            public String Title { get; set; }
            public String Permalink { get; set; }
            public String Content { get; set; }
            public Guid? SectionId { get; set; }
            public DateTime? PublishDate { get; set; }

            public List<SectionsController.SectionListing> AvailableSections;
        }

        #endregion View Models

    }
}
