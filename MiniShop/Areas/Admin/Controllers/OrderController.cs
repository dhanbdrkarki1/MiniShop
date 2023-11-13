using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MiniShop.DataAccess.Repository;
using MiniShop.DataAccess.Repository.IRepository;
using MiniShop.Models.Entity;
using MiniShop.Models.ViewModels;
using MiniShop.Utility;

namespace MiniShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Details(int orderId)
        {
            OrderVM orderVM = new()
            {
                Order = _unitOfWork.Order.Get(u => u.OrderId == orderId, includeProperties: "ApplicationUser"),
                OrderItem = _unitOfWork.OrderItem.GetAll(u => u.OrderId == orderId, includeProperties: "Product")
            };

            return View(orderVM);
        }

        [HttpPost]
        public IActionResult UpdateOrderDetails()
        {
            var orderFromDb = _unitOfWork.Order.Get(u => u.OrderId == OrderVM.Order.OrderId);
            orderFromDb.Name = OrderVM.Order.Name;
            orderFromDb.PhoneNumber = OrderVM.Order.PhoneNumber;
            orderFromDb.StreetAddress = OrderVM.Order.StreetAddress;
            orderFromDb.City = OrderVM.Order.City;
            orderFromDb.State = OrderVM.Order.State;
            orderFromDb.PostalCode = OrderVM.Order.PostalCode;

            _unitOfWork.Order.Update(orderFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Order details updated successfully.";



            return RedirectToAction(nameof(Details), new {orderId = orderFromDb.OrderId});
        }


        #region API Calls
        [HttpGet]
        public IActionResult GetAll(int id)
        {
            List<Order> objOrderList = _unitOfWork.Order.GetAll(includeProperties: "ApplicationUser").ToList();
            return Json(new { data = objOrderList });
        }


        #endregion

    }
}
