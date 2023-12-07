using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniShop.DataAccess.Repository;
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

        public ProductVM ProductVM { get; set; }
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
            // Calculate OverallRating for each product
            productCatalogVM.OverallRatings = new Dictionary<int, decimal>();
            foreach (var product in productCatalogVM.Products)
            {
                decimal overallRating = CalculateOverallRatingForProduct(product); 
                productCatalogVM.OverallRatings.Add(product.ProductId, overallRating); 
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

        private decimal CalculateOverallRatingForProduct(Product product)
        {
            var productReviews = _unitOfWork.ProductReview.GetAll(r => r.ProductId == product.ProductId);

            if (productReviews.Any())
            {
                decimal totalRating = productReviews.Sum(r => r.Rating);
                decimal averageRating = totalRating / productReviews.Count();

                return averageRating;
            }

            return 0m;
        }

        public IActionResult Details(int productId)
        {
            var product = _unitOfWork.Product.Get(u => u.ProductId == productId, includeProperties: "Category");
            var reviews = _unitOfWork.ProductReview
                .GetAll(r => r.ProductId == productId, includeProperties: "ApplicationUser").ToList();

            ProductReviewVM productReviewVM = new()
            {
                ProductReviewsList = reviews,
            };

            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.ProductId == productId, includeProperties: "Category"),
                Quantity = 1,
                ProductId = productId

            };

            // cacluating rating
            decimal overallRating = new Utils().CalculateOverallRating(reviews);

            int unitsSold = CalculateTotalUnitsSold(productId);

            ProductVM productVM = new ProductVM();
            productVM.ShoppingCart = cart;
            productVM.ProductReviewVM = productReviewVM;
            productVM.OverallRating = overallRating;
            productVM.UnitSold = unitsSold;

            if (User.Identity.IsAuthenticated)
            {

                bool hasPurchasedProduct = CheckIfUserHasPurchasedProduct(productId);
                if (hasPurchasedProduct)
                {

                    productVM.HasPurchasedProduct = CheckIfUserHasPurchasedProduct(productId);
                    return View(productVM);
                }
            }

            productVM.HasPurchasedProduct = false;
            return View(productVM);
        }


        //calculate the total units sold for a specific product based on delivered orders
        public int CalculateTotalUnitsSold(int productId)
        {
            var deliveredOrderItems = _unitOfWork.OrderItem
                .GetAll(oi => oi.Order.OrderStatus == "Delivered" && oi.Order.PaymentStatus == "Approved" && oi.ProductId == productId);

            int totalUnitsSold = deliveredOrderItems.Sum(oi => oi.Quantity);

            return totalUnitsSold;
        }


        // Check if the current user has purchased the product with the given ID
        public bool CheckIfUserHasPurchasedProduct(int productId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var currentUserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            bool hasPurchasedProduct = _unitOfWork.OrderItem
                .GetAll(item => item.Order.ApplicationUserId == currentUserId &&
                                item.Order.OrderStatus == "Delivered" && item.Order.PaymentStatus == "Approved" &&
                                item.ProductId == productId).Any();

            return hasPurchasedProduct;
        }

        public IActionResult SearchProduct(string? query)
        {
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [Authorize]
        public IActionResult Details(ProductVM productVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            productVM.ShoppingCart.ApplicationUserId = userId;
            productVM.ShoppingCart.Product = null;
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
        [HttpPost]
        public IActionResult PostReview(ProductVM productVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                var productReview = new ProductReview
                {
                    ApplicationUserId = userId,
                    ProductId = productVM.ShoppingCart.ProductId,
                    ReviewText = productVM.ProductReviewVM.ReviewText,
                    Rating = productVM.ProductReviewVM.Rating,
                    PublishedAt = DateTime.Now
                };


                _unitOfWork.ProductReview.Add(productReview);
                _unitOfWork.Save();

            }
            return RedirectToAction(nameof(Details), new { productId = productVM.ShoppingCart.ProductId });
        }

        [Authorize]
        public IActionResult BuyNow(int productId, int quantity)
        {
            return RedirectToAction("Checkout", "Cart", new { productId = productId, quantity=quantity });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}