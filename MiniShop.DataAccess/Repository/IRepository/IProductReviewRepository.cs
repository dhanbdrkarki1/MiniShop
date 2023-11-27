using MiniShop.Models.Entity;

namespace MiniShop.DataAccess.Repository.IRepository
{
    public interface IProductReviewRepository : IRepository<ProductReview>
    {
        void Update(ProductReview obj);
    }
}
