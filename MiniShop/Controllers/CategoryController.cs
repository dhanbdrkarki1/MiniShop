using Microsoft.AspNetCore.Mvc;
using MiniShop.Data;
using MiniShop.Models.Entity;
using System;

namespace MiniShop.Controllers
{
    public class CategoryController : Controller
    {
        //DEPENDENCY INJECTION
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            List<Category> categories = _db.Categories.ToList();

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
                _db.Categories.Add(category);
                _db.SaveChanges();
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

            Category? category = _db.Categories.Find(id);

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
                _db.Categories.Update(category);
                _db.SaveChanges();
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

            Category? category = _db.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }
            else
            {
                _db.Categories.Remove(category);
                _db.SaveChanges();
                TempData["success"] = "Category " + category.Name + " has been deleted successfully.";
                return RedirectToAction("Index");

            }
        }
    }
}
