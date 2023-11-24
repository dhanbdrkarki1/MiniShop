﻿using Microsoft.AspNetCore.Authorization;
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
            productCatalogVM.PaginationInfo = new PaginationInfo
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
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.ProductId == productId, includeProperties: "Category"),
                Quantity = 1,
                ProductId = productId

            };

            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);
            if (cartFromDb != null)
            {
                // update cart if product in cart exist
                cartFromDb.Quantity += shoppingCart.Quantity;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
                TempData["success"] = "Product updated in a cart successfully";
            }
            else
            {
                // add product to cart
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                //adding cart total no. in session
                HttpContext.Session.SetInt32(SD.Session_Cart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
                TempData["success"] = "Product added to cart successfully";
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
                TempData["success"] = "Product updated in the cart successfully";
            }
            else
            {
                // Add the product to the cart
                Product product = _unitOfWork.Product.Get(u => u.ProductId == productId);

                if (product != null)
                {
                    ShoppingCart cart = new ShoppingCart
                    {
                        Product = product,
                        Quantity = 1,
                        ApplicationUserId = userId
                    };

                    _unitOfWork.ShoppingCart.Add(cart);
                    TempData["success"] = "Product added to the cart successfully";
                }
                else
                {
                    TempData["error"] = "Product not found"; // Handle the case where the product doesn't exist
                }
            }
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}