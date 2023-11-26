using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniShop.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }

        ISubCategoryRepository SubCategory { get; }
        IProductRepository Product { get; }

        IShoppingCartRepository ShoppingCart { get; }

        IApplicationUserRepository ApplicationUser { get; }

        IOrderRepository Order { get; }
        IOrderItemRepository OrderItem { get; }

        IMyOrderRepository MyOrder { get; }
        void Save();
    }
}
