using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MiniShop.DataAccess.Repository.IRepository;
using MiniShop.Models.Entity;
using MiniShop.Models.ViewModels;
using MiniShop.Utility;
using System.Security.Claims;
using System.Xml;

namespace MiniShop.Areas.Customer.Controllers
{
    [Area("customer")]
    [Authorize]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.
                GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                Order = new()
            };

            ShoppingCart sp = new ShoppingCart();

            if (ShoppingCartVM.ShoppingCartList != null && ShoppingCartVM.ShoppingCartList.Any())
            {
                ShoppingCartVM.Order.OrderTotal = 0;
                foreach (var cart in ShoppingCartVM.ShoppingCartList)
                {
                    cart.Price = (double)cart.Product.Price;
                    // each product total
                    string itemCost = (cart.Price * cart.Quantity).ToString("0.00");
                    cart.Total = double.Parse(itemCost);
                    ShoppingCartVM.SubTotal += cart.Total;
                }
                ShoppingCartVM.IsCartEmpty = false;
            }
            else
            {
                ShoppingCartVM.IsCartEmpty = true;

            }

            double vatAmt = double.Parse(ShoppingCartVM.SubTotal.ToString("0.00")) * SD.VAT_Rate / 100;
            ShoppingCartVM.VATAmount = double.Parse(vatAmt.ToString("0.00"));
            //price after vat and delivery fee
            ShoppingCartVM.Order.OrderTotal = ShoppingCartVM.SubTotal + ShoppingCartVM.VATAmount + SD.Delivery_Fee;

            return View(ShoppingCartVM);
        }

        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == cartId);
            cartFromDb.Quantity += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == cartId, tracked: true);
            if (cartFromDb.Quantity <= 1)
            {
                //remove that from cart
                HttpContext.Session.SetInt32(SD.Session_Cart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);

                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Quantity -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == cartId, tracked: true);

            HttpContext.Session.SetInt32(SD.Session_Cart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);

            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Checkout(int? productId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser user = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            if (productId == null)
            {
                ShoppingCartVM = new()
                {
                    ShoppingCartList = _unitOfWork.ShoppingCart.
                    GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                    Order = new()
                };

                ShoppingCartVM.Order.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
                ShoppingCartVM.Order.Name = ShoppingCartVM.Order.ApplicationUser.Name;
                ShoppingCartVM.Order.PhoneNumber = ShoppingCartVM.Order.ApplicationUser.PhoneNumber;
                ShoppingCartVM.Order.StreetAddress = ShoppingCartVM.Order.ApplicationUser.StreetAddress;
                ShoppingCartVM.Order.City = ShoppingCartVM.Order.ApplicationUser.City;
                ShoppingCartVM.Order.State = ShoppingCartVM.Order.ApplicationUser.State;
                ShoppingCartVM.Order.PostalCode = ShoppingCartVM.Order.ApplicationUser.PostalCode;
                ShoppingCart sp = new ShoppingCart();
                foreach (var cart in ShoppingCartVM.ShoppingCartList)
                {
                    cart.Price = (double)cart.Product.Price;
                    // each product total
                    string itemCost = (cart.Price * cart.Quantity).ToString("0.00");
                    cart.Total = double.Parse(itemCost);
                    ShoppingCartVM.SubTotal += cart.Total;
                }

            }
            else
            {
                var product = _unitOfWork.Product.Get(u => u.ProductId == productId, includeProperties: "Category");
                double price = (double)product.Price;
                int quantity = 1;

                if (product == null)
                {
                    return NotFound();
                }


                ShoppingCartVM = new()
                {
                    ShoppingCartList = new List<ShoppingCart>(),
                    Order = new Order
                    {
                        ApplicationUser = user,
                        Name = user.Name,
                        PhoneNumber = user.PhoneNumber,
                        StreetAddress = user.StreetAddress,
                        City = user.City,
                        State = user.State,
                        PostalCode = user.PostalCode,
                    },
                };
                ShoppingCartVM.SubTotal = double.Parse(((double)price * quantity).ToString("0.00"));


                var cartItem = new ShoppingCart
                {
                    Product = product,
                    ApplicationUserId = userId,
                    ProductId = (int)productId,
                    Price = price,
                    Quantity = quantity
                };
                ((List<ShoppingCart>)ShoppingCartVM.ShoppingCartList).Add(cartItem);
            }

            double vatAmt = double.Parse(ShoppingCartVM.SubTotal.ToString("0.00")) * SD.VAT_Rate / 100;
            ShoppingCartVM.VATAmount = double.Parse(vatAmt.ToString("0.00"));
            //price after vat and delivery fee
            ShoppingCartVM.Order.OrderTotal = ShoppingCartVM.SubTotal + ShoppingCartVM.VATAmount + SD.Delivery_Fee;

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Checkout")]
        public IActionResult CheckoutPOST(int? productId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(
                u => u.ApplicationUserId == userId, includeProperties: "Product");

            ShoppingCartVM.Order.OrderDate = System.DateTime.Now;
            ShoppingCartVM.Order.ApplicationUserId = userId;


            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = (double)cart.Product.Price;
                // each product total
                cart.Total = cart.Price * cart.Quantity;
                ShoppingCartVM.SubTotal += (cart.Price * cart.Quantity);
            }
            double vatAmt = double.Parse(ShoppingCartVM.SubTotal.ToString("0.00")) * SD.VAT_Rate / 100;
            ShoppingCartVM.VATAmount = double.Parse(vatAmt.ToString("0.00"));
            //price after vat and delivery fee
            ShoppingCartVM.Order.OrderTotal = ShoppingCartVM.SubTotal + ShoppingCartVM.VATAmount + SD.Delivery_Fee;


            ShoppingCartVM.Order.PaymentStatus = SD.Payment_Status_Pending;
            ShoppingCartVM.Order.OrderStatus = SD.Status_Pending;

            _unitOfWork.Order.Add(ShoppingCartVM.Order);
            _unitOfWork.Save();


            // saving order item
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderItem orderItem = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = ShoppingCartVM.Order.OrderId,
                    Price = cart.Price,
                    Quantity = cart.Quantity

                };
                _unitOfWork.OrderItem.Add(orderItem);
                _unitOfWork.Save();
            }

            if (ShoppingCartVM.Order.PaymentMethod == SD.Payment_Method_Esewa)
            {
                if(productId  != null)
                {
                    HttpContext.Session.SetString("CartAccess", "Denied");
                }
                else
                {
                    HttpContext.Session.SetString("CartAccess", "Allowed");

                }
                return RedirectToAction(nameof(ESewaRequest), new { order_id = ShoppingCartVM.Order.OrderId, order_total = ShoppingCartVM.Order.OrderTotal, sub_total = ShoppingCartVM.SubTotal, tax_amt = ShoppingCartVM.VATAmount });
            }
            else
            {
                return RedirectToAction(nameof(OrderConfirmation), new { order_id = ShoppingCartVM.Order.OrderId });
            }

        }

        public IActionResult ESewaRequest(int order_id, double order_total, double sub_total, double tax_amt)
        {
            ViewBag.OrderId = order_id;
            ViewBag.OrderTotal = order_total;
            ViewBag.SubTotal = sub_total;
            ViewBag.VATAmount = tax_amt;
            return View();
        }

        public async Task<IActionResult> ESewaVerify(string oid, string amt, string tAmt, string txAmt, string pdc, string refId)
        {
            var url = "https://uat.esewa.com.np/epay/transrec";

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("amt", amt),
                new KeyValuePair<string, string>("txAmt", txAmt),
                new KeyValuePair<string, string>("pdc", pdc),
            new KeyValuePair<string, string>("amt", tAmt),
            new KeyValuePair<string, string>("scd", "EPAYTEST"),
            new KeyValuePair<string, string>("rid", refId),
            new KeyValuePair<string, string>("pid", oid)
        });

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(responseContent);
                    string status = xmlDoc.DocumentElement.InnerText.Trim('\n', ' ');
                    Order orderObj = _unitOfWork.Order.Get(o => o.OrderId == int.Parse(oid));


                    if (status == "Success" && orderObj != null)
                    {


                        //removing cart
                        var claimsIdentity = (ClaimsIdentity)User.Identity;
                        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                        var cartItemsToRemove = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId);

                        //dont empty card if request is not from buy now 
                        if (HttpContext.Session.GetString("CartAccess") == "Allowed")
                        {
                            _unitOfWork.ShoppingCart.Remove(cartItemsToRemove);
                        }
                        orderObj.PaymentStatus = SD.Payment_Status_Approved;
                        _unitOfWork.Order.Update(orderObj);
                        _unitOfWork.Save();
                        HttpContext.Session.Clear();

                        return RedirectToAction(nameof(OrderConfirmation), new { order_id = oid });
                    }
                    else
                    {
                        return RedirectToAction(nameof(ESewaRequest), new { order_id = oid, order_total = amt });
                    }
                }
                else
                {
                    // Handle the scenario where the HTTP request to eSewa fails
                    return RedirectToAction(nameof(Index));
                }
            }
        }


        public IActionResult OrderConfirmation(int order_id)
        {
            HttpContext.Session.Clear();

            return View(order_id);
        }

    }
}
