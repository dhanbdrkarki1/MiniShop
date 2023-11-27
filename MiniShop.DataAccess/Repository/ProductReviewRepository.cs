using MiniShop.DataAccess.Data;
using MiniShop.DataAccess.Repository.IRepository;
using MiniShop.Models.Entity;

namespace MiniShop.DataAccess.Repository
{
    public class ProductReviewRepository : Repository<ProductReview>, IProductReviewRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductReviewRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;

        }

        public void Update(ProductReview obj)
        {
            _db.ProductReviews.Update(obj);
        }
    }
}
