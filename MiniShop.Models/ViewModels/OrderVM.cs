using Microsoft.AspNetCore.Mvc.Rendering;
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
        public IEnumerable<SelectListItem> PaymentStatusList  { get; set; }

        public IEnumerable<SelectListItem> OrderStatusList { get; set; }

        public IEnumerable<SelectListItem> PaymentMethodList { get; set; }

    }
}
