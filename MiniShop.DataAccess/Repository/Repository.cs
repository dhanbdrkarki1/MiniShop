using Microsoft.EntityFrameworkCore;
using MiniShop.DataAccess.Data;
using System.Linq.Expressions;

namespace MiniShop.DataAccess.Repository.IRepository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        //DEPENDENCY INJECTION
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>(); // equivalent to _db.Categories = dbSet
            _db.Products.Include(u => u.Category).Include(u => u.CategoryId);
            _db.Products.Include(u => u.SubCategory).Include(u => u.SubCategoryId);
            _db.ProductReviews.Include(u => u.ApplicationUser).Include(u => u.ApplicationUserId);
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> query;

			if (tracked)
            {
				query = dbSet;
			}
            else
            {
				query = dbSet.AsNoTracking();
			}
			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var includeProp in includeProperties
					.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProp);
				}
			}
			query = query.Where(filter);
			return query.FirstOrDefault();
		}

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if(filter  != null)
            {
                query = query.Where(filter);

            }
            if (!string.IsNullOrEmpty(includeProperties)){
                foreach(var includeProp in includeProperties
                    .Split(new char[] {',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
