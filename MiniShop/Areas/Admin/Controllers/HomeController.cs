using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class HomeController : Controller
    {
        //DEPENDENCY INJECTION
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            int totalOrders = _unitOfWork.Order.GetAll().Count();
            int totalOrderDelivered = _unitOfWork.Order.GetAll(o => o.OrderStatus == SD.Status_Delivered).Count();
            int totalOrderPending = _unitOfWork.Order.GetAll(o => o.OrderStatus == SD.Status_Pending).Count();
            int totalOrderSentForDelivery = _unitOfWork.Order.GetAll(o => o.OrderStatus == SD.Status_Sent_For_Delivery).Count();
            AdminDashboardVM adminDashboardVM = new()
            {
                totalOrders = totalOrders,
                totalOrderDelivered = totalOrderDelivered,
                totalOrderPending = totalOrderPending,
                totalOrderSentForDelivery = totalOrderSentForDelivery
            };
            return View(adminDashboardVM);
        }
    }
}
