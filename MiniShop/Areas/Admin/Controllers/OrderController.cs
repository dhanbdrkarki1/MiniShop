using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                OrderItem = _unitOfWork.OrderItem.GetAll(u => u.OrderId == orderId, includeProperties: "Product"),

            };

            List<SelectListItem> paymentStatusList = new List<SelectListItem>
            {
                new SelectListItem { Text = SD.Payment_Status_Pending, Value = SD.Payment_Status_Pending},
                new SelectListItem { Text = SD.Payment_Status_Approved, Value = SD.Payment_Status_Approved },
                new SelectListItem { Text = SD.Payment_Status_Rejected, Value = SD.Payment_Status_Rejected }
            };

            List<SelectListItem> orderStatusList = new List<SelectListItem>
            {
                new SelectListItem { Text = SD.Status_Pending, Value = SD.Status_Pending},
                new SelectListItem { Text = SD.Status_Sent_For_Delivery, Value = SD.Status_Sent_For_Delivery },
                new SelectListItem { Text = SD.Status_Delivered, Value = SD.Status_Delivered },
                new SelectListItem { Text = SD.Status_Cancelled, Value = SD.Status_Cancelled }
            };

            List<SelectListItem> paymentMethodList = new List<SelectListItem>
            {
                new SelectListItem { Text = SD.Payment_Method_Cash, Value = SD.Payment_Method_Cash},
                new SelectListItem { Text = SD.Payment_Method_Esewa, Value = SD.Payment_Method_Esewa },
            };

            string currentPaymentStatus = orderVM.Order.PaymentStatus;
            string currentOrderStatus = orderVM.Order.OrderStatus;
            string currentPaymentMethod = orderVM.Order.PaymentMethod;

            foreach (var status in paymentStatusList)
            {
                if (status.Value == currentPaymentStatus)
                {
                    status.Selected = true;
                    
                    break; 
                }
            }

            foreach (var status in orderStatusList)
            {
                if (status.Value == currentOrderStatus)
                {
                    status.Selected = true;

                    break;
                }
            }

            foreach (var payment in paymentMethodList)
            {
                if(payment.Value == currentPaymentMethod)
                {
                    payment.Selected = true;
                    break;
                }
            }

            orderVM.PaymentStatusList = paymentStatusList;
            orderVM.OrderStatusList = orderStatusList;
            orderVM.PaymentMethodList = paymentMethodList;
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
            orderFromDb.PaymentStatus = OrderVM.Order.PaymentStatus;
            orderFromDb.OrderStatus = OrderVM.Order.OrderStatus;
            orderFromDb.PaymentMethod = OrderVM.Order.PaymentMethod;
            _unitOfWork.Order.Update(orderFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Order details updated successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderFromDb.OrderId });
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
