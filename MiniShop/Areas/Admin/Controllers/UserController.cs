using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniShop.DataAccess.Data;
using MiniShop.DataAccess.Repository;
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
    public class UserController : Controller
    {
        //DEPENDENCY INJECTION
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string userId)
        {
            if (userId == null || string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            var userRoles = _userManager.GetRolesAsync(user).GetAwaiter().GetResult();
            var userRole = userRoles.FirstOrDefault();

            RoleManagementVM roleManagementVM = new RoleManagementVM()
            {
                ApplicationUser = user,
                RoleList = _roleManager.Roles.Select(role => new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Name,
                    Selected = (role.Name == userRole)
                })
            };

            return View(roleManagementVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            var applicationUser = _userManager.FindByIdAsync(roleManagementVM.ApplicationUser.Id).GetAwaiter().GetResult();
            var oldRoles = _userManager.GetRolesAsync(applicationUser).GetAwaiter().GetResult();
            var oldRole = oldRoles.FirstOrDefault();

            if (roleManagementVM.ApplicationUser.Role != oldRole)
            {
                _userManager.RemoveFromRolesAsync(applicationUser, oldRoles).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }

            return RedirectToAction("Index");
        }




        # region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _unitOfWork.ApplicationUser.GetAll().ToList();

            foreach (var user in objUserList)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
            }

            return Json(new { data = objUserList });
        }


        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while locking/unlocking" });
            }
            // unlocking user if they are locked
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(50);
            }
            _unitOfWork.ApplicationUser.Update(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Operation successfull." });
        }


        #endregion

    }
}
