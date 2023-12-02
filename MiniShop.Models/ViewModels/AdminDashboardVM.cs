using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniShop.Models.ViewModels
{
    public class AdminDashboardVM
    {
        public int totalOrders { get; set; }
        public int totalOrderDelivered { get; set; }
        public int totalOrderPending { get; set; }
        public int totalOrderSentForDelivery { get; set;}
    }
}
