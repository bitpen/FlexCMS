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
            if (!String.IsNullOrEmpty(article.PublishDate))
            {
                var publishDate = DateTime.Parse(article.PublishDate);

                
                var hour = article.PublishHour;
                var minute = article.PublishMinute;
                var period = article.PublishPeriod;

                if (hour != null && minute != null && period != null)
                {
                    if (period == 2)
                    {
                        hour += 12;
                    }

                    publishDate = new DateTime(publishDate.Year, publishDate.Month, publishDate.Day, (int)hour, (int)minute, 0);
                }
                
                add.PublishDate_utc = publishDate.ToUniversalTime();
            }
            
             ValidationErrors<ArticlesBO.AddArticleBLM.ValidatableFields, String> errors;
            using (var uow = new UnitOfWork("jt"))
            {
                var articleBO = new ArticlesBO(uow);
               
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

            var mappedErrors = errors.Map(_addArticleValidationMapper);
            foreach (var error in mappedErrors.ToList())
            {
                ModelState.AddModelError(error.Key, error.Value);
            }


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
                var localDate = article.PublishDate_utc.Value.ToLocalTime();
                view.PublishDate = String.Format("{0:yyyy-MM-dd}", localDate);
                view.PublishHour = localDate.Hour;
                view.PublishMinute = localDate.Minute;
                if (localDate.Hour > 12)
                {
                    view.PublishPeriod = 2;
                    view.PublishHour -= 12;
                }
                else
                {
                    view.PublishPeriod = 1;
                }
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
            if (!String.IsNullOrEmpty(article.PublishDate))
            {
                var publishDate = DateTime.Parse(article.PublishDate);


                var hour = article.PublishHour;
                var minute = article.PublishMinute;
                var period = article.PublishPeriod;

                if (hour != null && minute != null && period != null)
                {
                    if (period == 2)
                    {
                        hour += 12;
                    }

                    publishDate = new DateTime(publishDate.Year, publishDate.Month, publishDate.Day, (int)hour, (int)minute, 0);
                }

                update.PublishDate_utc = publishDate.ToUniversalTime();
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
            private static List<int> _hours;
            private static List<int> _minutes;
            private static Dictionary<int, string> _periods;

            public NewArticle()
            {
                AvailableSections = new List<SectionsController.SectionListing>();
            }

            static NewArticle()
            {
                _hours = new List<int>();
                for (int i = 1; i <= 12; i++)
                {
                    _hours.Add(i);
                }

                _minutes = new List<int>();
                _minutes.Add(00);
                _minutes.Add(15);
                _minutes.Add(30);
                _minutes.Add(45);

                _periods = new Dictionary<int, string>();
                _periods.Add(1, "AM");
                _periods.Add(2, "PM");
            }

            public String Title { get; set; }
            public String Permalink { get; set; }
            public String Content { get; set; }
            public Guid? SectionId { get; set; }
            public String PublishDate { get; set; }
            public int? PublishHour { get; set; }
            public int? PublishMinute { get; set;}
            public int? PublishPeriod { get; set; } 

            public List<SectionsController.SectionListing> AvailableSections { get; set;}
            public List<int> AvailableHours
            {
                get
                {
                    return _hours;
                }
            }
            public List<int> AvailableMinutes
            {
                get
                {
                    return _minutes;
                }
            }
            public Dictionary<int, string> AvailablePeriods
            {
                get
                {
                    return _periods;
                }
            }
        }

        #endregion View Models

        #region Validation Mappers

        private static Dictionary<ArticlesBO.AddArticleBLM.ValidatableFields, string> _addArticleValidationMapper;

        static ArticlesController(){
            _addArticleValidationMapper = new Dictionary<ArticlesBO.AddArticleBLM.ValidatableFields, string>();
            _addArticleValidationMapper.Add(ArticlesBO.AddArticleBLM.ValidatableFields.General, string.Empty);
            _addArticleValidationMapper.Add(ArticlesBO.AddArticleBLM.ValidatableFields.Title, "Title");
            _addArticleValidationMapper.Add(ArticlesBO.AddArticleBLM.ValidatableFields.Alias, "Permalink");
            _addArticleValidationMapper.Add(ArticlesBO.AddArticleBLM.ValidatableFields.SectionId, "SectionId");
        }

        #endregion

    }
}
