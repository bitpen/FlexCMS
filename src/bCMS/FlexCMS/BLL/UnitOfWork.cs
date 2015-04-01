using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlexCMS.BLL
{
    public partial class UnitOfWork : IDisposable
    {
        private CmsContext _cmsContext;



        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userName"></param>
        public UnitOfWork(String userName)
        {
            _cmsContext = new CmsContext(userName);
        }

        

        public void Dispose()
        {
            _cmsContext.Dispose();
        }

        public CmsContext GetCmsContext()
        {
            return _cmsContext;
        }
    }
}