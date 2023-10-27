using Microsoft.AspNetCore.Mvc;
using MiniShop.DataAccess.Data;
using MiniShop.DataAccess.Repository.IRepository;
using MiniShop.Models.Entity;
using System;

namespace MiniShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        //DEPENDENCY INJECTION
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();

            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                category.ModifiedDate = DateTime.Now;
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                TempData["success"] = "New category created successfully.";
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? category = _unitOfWork.Category.Get(u => u.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                category.ModifiedDate = DateTime.Now;
                _unitOfWork.Category.Update(category);
                _unitOfWork.Save();
                TempData["success"] = "Category " + category.Name + " has been updated successfully.";
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? category = _unitOfWork.Category.Get(u => u.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }
            else
            {
                _unitOfWork.Category.Remove(category);
                _unitOfWork.Save();
                TempData["success"] = "Category " + category.Name + " has been deleted successfully.";
                return RedirectToAction("Index");

            }
        }
    }
}
