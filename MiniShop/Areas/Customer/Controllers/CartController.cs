using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniShop.DataAccess.Repository.IRepository;
using MiniShop.Models.Entity;
using MiniShop.Models.ViewModels;
using MiniShop.Utility;
using System.Security.Claims;

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
                    cart.Total = cart.Price * cart.Quantity;
                    ShoppingCartVM.Order.OrderTotal += (cart.Price * cart.Quantity);
                }
                ShoppingCartVM.IsCartEmpty = false;
            }
            else
            {
                ShoppingCartVM.IsCartEmpty = true;

            }

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
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == cartId, tracked:true);
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

        public IActionResult Checkout()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

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
                cart.Total = cart.Price * cart.Quantity;
                ShoppingCartVM.Order.OrderTotal += (cart.Price * cart.Quantity);
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Checkout")]
        public IActionResult CheckoutPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(
                u => u.ApplicationUserId == userId, includeProperties: "Product");

            ShoppingCartVM.Order.OrderDate = System.DateTime.Now;
            ShoppingCartVM.Order.ApplicationUserId = userId;


            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);


            ShoppingCart sp = new ShoppingCart();
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = (double)cart.Product.Price;
                // each product total
                cart.Total = cart.Price * cart.Quantity;
                ShoppingCartVM.Order.OrderTotal += (cart.Price * cart.Quantity);
            }
            ShoppingCartVM.Order.PaymentMethod = SD.Payment_Method_Cash;
            ShoppingCartVM.Order.PaymentStatus = SD.Payment_Status_Approved;
            ShoppingCartVM.Order.OrderStatus = SD.Status_Approved;

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

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.Order.OrderId });
        }

        public IActionResult OrderConfirmation(int id)
        {
            HttpContext.Session.Clear();

            return View(id);
        }

    }
}
