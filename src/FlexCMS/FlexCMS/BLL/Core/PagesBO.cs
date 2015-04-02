using System;
using System.Collections.Generic;
using System.Linq;
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

            errors = page.CreateValidationErrorsCollection();

            return id;
        }
    }
}