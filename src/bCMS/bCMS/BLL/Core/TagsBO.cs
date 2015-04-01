using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

namespace bCMS.BLL.Core
{
    /// <summary>
    /// Business Layer object for interacting with the datastore 
    /// in relation to Tags
    /// </summary>
    public class TagsBO
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
        public TagsBO(UnitOfWork uow)
        {
            if (uow == null)
            {
                throw new ArgumentNullException("uow", "Valid UnitOfWork required.");
            }
            _uow = uow;
            _cmsContext = _uow.GetCmsContext();
        }

        /// <summary>
        /// Add a new unique tag to the datastore.  If it already exists, no new tag will be created.
        /// Tags are case insensitive
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException">When name is empty or null</exception>
        /// <returns>Primary key of the newly created tag</returns>
        public Guid? Add(string name)
        {
            Guid? id = null;

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "Name cannot be null");
            }

            var cleanedName = name.Trim();
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "Name cannot be null");
            }

            var tag = _cmsContext.Tags.FirstOrDefault(i => i.Name.ToUpper().Equals(cleanedName.ToUpper()));
            if (tag != null)
            {
                return tag.Id;
            }

            tag = new Models.Core.Tag();
            tag.Id = Guid.NewGuid();
            tag.Name = cleanedName;
            using (var transaction = new TransactionScope())
            {
                _cmsContext.Tags.Add(tag);
                _cmsContext.SaveChanges();
                id = tag.Id;
                transaction.Complete();
            }

            return id;
        }

        /// <summary>
        /// Update the name of an existing tag in the datastore.
        /// Tags are case insensitive
        /// </summary>
        /// <param name="tagId">Primary key of the tag</param>
        /// <param name="name">Updated name</param>
        /// <exception cref="ArgumentNullException">When name is empty or null</exception>
        public void Update(Guid tagId, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "Name cannot be null");
            }

            var cleanedName = name.Trim();
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "Name cannot be null");
            }

            using (var transaction = new TransactionScope())
            {
                var tag = _cmsContext.Tags.Find(tagId);
                tag.Name = cleanedName;
                _cmsContext.Entry(tag).State = System.Data.Entity.EntityState.Modified;
                _cmsContext.SaveChanges();
                transaction.Complete();
            }
        }
    }
}