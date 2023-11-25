using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniShop.Utility
{
    public static class SD
    {
        public const string Role_Customer = "Customer";
		public const string Role_Admin = "Admin";

        public const string Status_Pending = "Pending";
        public const string Status_Approved = "Approved";
        public const string Status_Cancelled = "Cancelled";


        public const string Payment_Method_Cash = "Cash on Delivery";
        public const string Payment_Method_Esewa = "Esewa";

        public const string Payment_Status_Pending = "Pending";
        public const string Payment_Status_Approved = "Approved";
        public const string Payment_Status_Rejected = "Rejected";

        public const string Session_Cart = "SessionShoppingCart";


        public const int Delivery_Fee = 2;
        public const int VAT_Rate = 13;


    }
}
