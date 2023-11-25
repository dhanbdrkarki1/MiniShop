using MiniShop.Models.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniShop.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }

        public Order Order { get; set; }

        public bool IsCartEmpty { get; set; }

        public double SubTotal { get; set; }

        public double VATAmount { get; set; }
        public double TotalPrice { get; set; }

    }
}
