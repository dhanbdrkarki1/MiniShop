using MiniShop.DataAccess.Data;
using MiniShop.DataAccess.Repository.IRepository;
using MiniShop.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MiniShop.DataAccess.Repository
{
    public class SubCategoryRepository : Repository<SubCategory>, ISubCategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public SubCategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;

        }


        public void Update(SubCategory obj)
        {
            _db.SubCategories.Update(obj);
        }
    }
}
