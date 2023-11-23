using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniShop.DataAccess.Data;
using MiniShop.DataAccess.Repository.IRepository;
using MiniShop.Models.Entity;
using MiniShop.Models.ViewModels;
using MiniShop.Utility;
using System;

namespace MiniShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class SubCategoryController : Controller
    {
        //DEPENDENCY INJECTION
        private readonly IUnitOfWork _unitOfWork;
        public SubCategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<SubCategory> subCategories = _unitOfWork.SubCategory.GetAll(includeProperties: "Category").ToList();
            return View(subCategories);
        }

        public IActionResult Create()
        {
            SubCategoryVM subCategoryVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().
                        Select(u => new SelectListItem
                        {
                            Text = u.Name,
                            Value = u.CategoryId.ToString()
                        }),
                SubCategory = new SubCategory()
            };
            return View(subCategoryVM);
        }

        [HttpPost]
        public IActionResult Create(SubCategoryVM subCategoryVM)
        {
            if (ModelState.IsValid)
            {
                subCategoryVM.SubCategory.ModifiedDate = DateTime.Now;
                subCategoryVM.SubCategory.CreatedDate = DateTime.Now;
                _unitOfWork.SubCategory.Add(subCategoryVM.SubCategory);
                _unitOfWork.Save();
                TempData["success"] = "New Sub category created successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                subCategoryVM.CategoryList = _unitOfWork.Category.GetAll().
                        Select(u => new SelectListItem
                        {
                            Text = u.Name,
                            Value = u.CategoryId.ToString()
                        });
                return View(subCategoryVM);
            }

        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            SubCategoryVM subCategoryVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().
            Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.CategoryId.ToString()
            }),
                SubCategory = new SubCategory()
            };

            if (subCategoryVM.SubCategory == null)
            {
                return NotFound();
            }
            subCategoryVM.SubCategory = _unitOfWork.SubCategory.Get(u => u.SubCategoryId == id);
            return View(subCategoryVM);
        }

        [HttpPost]
        public IActionResult Edit(SubCategoryVM subCategoryVM)
        {
            if (ModelState.IsValid)
            {
                SubCategory subCategory = _unitOfWork.SubCategory.Get(u => u.SubCategoryId == subCategoryVM.SubCategory.SubCategoryId);

                subCategoryVM.SubCategory.ModifiedDate = DateTime.Now;
                subCategoryVM.SubCategory.CreatedDate = subCategory.CreatedDate;
                _unitOfWork.SubCategory.Update(subCategoryVM.SubCategory);
                _unitOfWork.Save();
                TempData["success"] = "New Sub category updated successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                subCategoryVM.CategoryList = _unitOfWork.Category.GetAll().
                        Select(u => new SelectListItem
                        {
                            Text = u.Name,
                            Value = u.CategoryId.ToString()
                        });
                return View(subCategoryVM);
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            SubCategory? subCategory = _unitOfWork.SubCategory.Get(u => u.SubCategoryId == id);

            if (subCategory == null)
            {
                return NotFound();
            }
            else
            {
                _unitOfWork.SubCategory.Remove(subCategory);
                _unitOfWork.Save();
                TempData["success"] = "Sub-category " + subCategory.Name + " has been deleted successfully.";
                return RedirectToAction("Index");

            }
        }

    }
}
