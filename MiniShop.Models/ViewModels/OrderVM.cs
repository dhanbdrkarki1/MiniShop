using MiniShop.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniShop.Models.ViewModels
{
    public class OrderVM
    {
        public Order Order { get; set; }
        public IEnumerable<OrderItem> OrderItem { get; set; }
    }
}
