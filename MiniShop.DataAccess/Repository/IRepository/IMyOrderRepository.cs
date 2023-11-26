using MiniShop.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniShop.DataAccess.Repository.IRepository
{
    public interface IMyOrderRepository : IRepository<Order>
    {
        void Update(Order obj);
    }
}
