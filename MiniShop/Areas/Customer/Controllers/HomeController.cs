using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniShop.DataAccess.Repository.IRepository;
using MiniShop.Models;
using MiniShop.Models.Entity;
using MiniShop.Models.ViewModels;
using MiniShop.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace MiniShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int? subCategoryId, int page = 1, int pageSize = 6)
        {
            var productCatalogVM = new ProductCatalogVM
            {
                CategoryList = _unitOfWork.Category.GetAll()
                    .Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.CategoryId.ToString()
                    }),
                SubCategoryList = _unitOfWork.SubCategory.GetAll().ToList()
            };

            productCatalogVM.Products = _unitOfWork.Product.GetAll(includeProperties: "Category,SubCategory").ToList();

            if (subCategoryId != null)
            {
                productCatalogVM.Products = _unitOfWork.Product.GetAll(u => u.SubCategoryId == subCategoryId).ToList();
            }

            int totalProducts = productCatalogVM.Products.Count();

            var paginatedProducts = productCatalogVM.Products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            productCatalogVM.Products = paginatedProducts;
            productCatalogVM.PaginationInfo = new()
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = totalProducts,
                UrlParams = new Dictionary<string, string> { { "subCategoryId", subCategoryId.ToString() } }
            };

            return View(productCatalogVM);
        }



        public IActionResult Details(int productId)
        {
            var product = _unitOfWork.Product.Get(u => u.ProductId == productId, includeProperties: "Category");
            var reviews = _unitOfWork.ProductReview.GetAll(r => r.ProductId == productId).ToList();

            ProductReviewVM productReviewVM = new()
            {
                Title = product.Name, 
                ProductReviewsList = reviews,
                ProductId = productId
            };

            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.ProductId == productId, includeProperties: "Category"),
                Quantity = 1,
                ProductId = productId

            };

            ProductVM productVM = new()
            {
                ShoppingCart = cart,
                ProductReviewVM = productReviewVM
            };
            return View(productVM);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ProductVM productVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            productVM.ShoppingCart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId && u.ProductId == productVM.ShoppingCart.ProductId);
            if (cartFromDb != null)
            {
                // update cart if product in cart exist
                cartFromDb.Quantity += productVM.ShoppingCart.Quantity;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
                TempData["success"] = "Product updated in a cart successfully.";
            }
            else
            {
                // add product to cart
                _unitOfWork.ShoppingCart.Add(productVM.ShoppingCart);
                _unitOfWork.Save();
                //adding cart total no. in session
                HttpContext.Session.SetInt32(SD.Session_Cart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
                TempData["success"] = "Product added to cart successfully.";
            }
            return RedirectToAction(nameof(Index));
        }


        // Add product to cart
        [Authorize]
        public IActionResult AddToCart(int productId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId && u.ProductId == productId);

            if (cartFromDb != null)
            {
                // Update cart if the product is already in the cart
                cartFromDb.Quantity += 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                TempData["success"] = "Product updated in the cart successfully.";
            }
            else
            {
                // Add the product to the cart
                Product product = _unitOfWork.Product.Get(u => u.ProductId == productId);
                ShoppingCart cart = new ShoppingCart
                {
                    ProductId = productId,
                    Quantity = 1,
                    ApplicationUserId = userId
                };
                _unitOfWork.ShoppingCart.Add(cart);
                TempData["success"] = "Product added to the cart successfully.";
            }
            _unitOfWork.Save();
            //adding cart total no. in session
            HttpContext.Session.SetInt32(SD.Session_Cart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            TempData["success"] = "Product added to cart successfully.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult BuyNow(int productId)
        {
            return RedirectToAction("Checkout", "Cart", new { productId = productId});
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}