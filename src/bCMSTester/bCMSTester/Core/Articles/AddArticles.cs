using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using bCMS.BLL;
using bCMS.BLL.Core;

namespace bCMSTester
{
    [TestClass]
    public class AddArticles
    {
        private Guid? articleId;

        /// <summary>
        /// Add a new valid, unpublished article
        /// </summary>
        [TestMethod]
        public void AddValidUnpublishedArticle()
        {
            var article = new bCMS.BLL.Core.ArticlesBO.AddArticleBLM();
            article.Title = "First Article";
            article.Alias = "First-Article";

            Guid? id;
            ArticlesBO.AddArticleBLM.ValidationErrors errors;
            using (var uow = new UnitOfWork("bCMSTester"))
            {
                var bo = new ArticlesBO(uow);
                 id = bo.Add(article, out errors);
            }
           
            if (errors.Count() > 0)
            {
                foreach(var set in errors.ToLookup()){
                    var field = set.Key;
                    foreach (var error in set)
                    {
                        Assert.Fail(field + ":" + error);
                    }
                }
                
            }
            Assert.AreNotEqual((Guid?)null, id, "Article Id not returned");
            articleId = id;
        }
    }
}
