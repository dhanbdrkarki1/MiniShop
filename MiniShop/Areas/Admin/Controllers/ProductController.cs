using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniShop.DataAccess.Data;
using MiniShop.DataAccess.Repository.IRepository;
using MiniShop.Models.Entity;
using MiniShop.Models.ViewModels;
using MiniShop.Utility;
using System;
using System.Data;

namespace MiniShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        //DEPENDENCY INJECTION
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties:"Category,SubCategory").ToList();

            return View(products);
        }

        public IActionResult ShowSubCategoryProduct(int subCategoryId)
        {
            return RedirectToAction(nameof(Index), new { subCategoryId });
        }

        [HttpGet]
        public IActionResult GetSubCategoriesByCategoryId(int categoryId)
        {
            var subCategories = _unitOfWork.SubCategory.GetAll(u => u.CategoryId == categoryId)
                                    .Select(u => new SelectListItem
                                    {
                                        Text = u.Name,
                                        Value = u.SubCategoryId.ToString()
                                    });
            return Json(subCategories);
        }


        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().
                                    Select(u => new SelectListItem
                                    {
                                        Text = u.Name,
                                        Value = u.CategoryId.ToString()
                                    }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                // create
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(u => u.ProductId ==  id);
                productVM.SubCategoryList = _unitOfWork.SubCategory.GetAll(u => u.SubCategoryId == productVM.Product.SubCategoryId).
                                    Select(u => new SelectListItem
                                    {
                                        Text = u.Name,
                                        Value = u.SubCategoryId.ToString()
                                    });
                return View(productVM );
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\products");

                    if(!string.IsNullOrEmpty(productVM.Product.ImageURL))
                    {
                        // delete old image
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageURL.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fs = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fs);
                    }
                    productVM.Product.ImageURL = @"\images\products\" + fileName;
                }
                if (productVM.Product.ProductId == 0)
                {
                    productVM.Product.CreatedDate = DateTime.Now;
                    productVM.Product.ModifiedDate = DateTime.Now;
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "New product created successfully.";
                }
                else
                {
                    Product product = _unitOfWork.Product.Get(u => u.ProductId == productVM.Product.ProductId);
                    productVM.Product.CreatedDate = _unitOfWork.Product.Get(u => u.ProductId == productVM.Product.ProductId).CreatedDate;
                    productVM.Product.ModifiedDate = DateTime.Now;
                    _unitOfWork.Product.Update(productVM.Product);
                    TempData["success"] = "Product updated successfully.";

                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().
                        Select(u => new SelectListItem
                        {
                            Text = u.Name,
                            Value = u.CategoryId.ToString()
                        });
                return View(productVM);
            }

        }


        # region API Calls
        [HttpGet]
        public IActionResult GetAll(int id) {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category,SubCategory").ToList();
            return Json(new { data = objProductList });
        }


        [HttpDelete]
        public IActionResult Delete(int? id) {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.ProductId == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageURL.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Product " + productToBeDeleted.Name + " has been deleted successfully." });

        }


        #endregion

    }
}
