using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProject0.DataAccess.Repository.IRepository;
using MyProject0.Models;
using MyProject0.Models.ViewModels;
using MyProject0.Utility;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace MyProject0.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM orderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
        }
        public IActionResult Index()
		{
			return View();
		}

        // The Id that we are accepting here is the OrderHeader id
        public IActionResult Details(int id)
        {
            orderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == id, "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == id, "Product").ToList()
            };
            return View(orderVM);
        }

        [HttpPost]
        [ActionName("Details")]
        public IActionResult Details_Pay_Now()
        {
            // In case we lost some data, we are populating everything again.
            orderVM.OrderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == orderVM.OrderHeader.Id, "ApplicationUser");
            orderVM.OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == orderVM.OrderHeader.Id, "Product").ToList();

            // All the stripe logic again
            var baseUrl = "https://localhost:44381/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = baseUrl + $"Admin/Order/PaymentConfirmation?OrderHeaderId={orderVM.OrderHeader.Id}",
                CancelUrl = baseUrl + $"Admin/Order/details?id={orderVM.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var items in orderVM.OrderDetail)
            {
                var SessionLineItems = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(items.Price * 100), // Here we have to give the price in long i.e. 20.50 => 2050
                        Currency = "USD",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = items.Product.Title
                        }
                    },
                    Quantity = items.Count
                };
                options.LineItems.Add(SessionLineItems);
            }

            var service = new Stripe.Checkout.SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentId(orderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);
        }

        public IActionResult PaymentConfirmation(int OrderHeaderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == OrderHeaderId);

            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                // This is a Company Order.
                var service = new SessionService();

                var session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(OrderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(OrderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }
            return View(OrderHeaderId);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin +","+SD.Role_Employee)]
        public IActionResult UpdateOrderDetails()
        {
            var OrderHeaderFromDb = _unitOfWork.OrderHeader.Get(x => x.Id == orderVM.OrderHeader.Id,"ApplicationUser");

            OrderHeaderFromDb.Name = orderVM.OrderHeader.Name;
            OrderHeaderFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
            OrderHeaderFromDb.StreetAddress = orderVM.OrderHeader.StreetAddress;
            OrderHeaderFromDb.City = orderVM.OrderHeader.City;
            OrderHeaderFromDb.State = orderVM.OrderHeader.State;
            OrderHeaderFromDb.PostalCode = orderVM.OrderHeader.PostalCode;

            if (!string.IsNullOrEmpty(orderVM.OrderHeader.Carrier))
            {
                OrderHeaderFromDb.Carrier = orderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
            {
                OrderHeaderFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeader.Update(OrderHeaderFromDb);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully";
            
            return RedirectToAction(nameof(Details),new {id = OrderHeaderFromDb.Id});
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully";
            return RedirectToAction(nameof(Details),new {id = orderVM.OrderHeader.Id});
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == orderVM.OrderHeader.Id);
            orderHeader.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = orderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            // If it's a company order then add the Payment Due Date.
            if(orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
            }

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["Success"] = "Order Shipped Successfully";
            return RedirectToAction(nameof(Details), new { id = orderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get( x => x.Id == orderVM.OrderHeader.Id);

            if(orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                // This means that the payment has been made and we need to give a refund
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["Success"] = "Order Cancelled Successfully";
            return RedirectToAction(nameof(Details), new { id = orderVM.OrderHeader.Id });
        }

        [HttpGet]
		public IActionResult GetAll(string status)
		{
            IEnumerable<OrderHeader> orderHeader;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Admin))
            {
                orderHeader = _unitOfWork.OrderHeader.GetAll(IncludeProperties: "ApplicationUser").ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                orderHeader = _unitOfWork.OrderHeader.GetAll(x=> x.ApplicationUserId == userId, IncludeProperties: "ApplicationUser").ToList();
            }

            switch (status)
            {
                case "pending":
                    orderHeader = orderHeader.Where(x => x.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeader = orderHeader.Where(x => x.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeader = orderHeader.Where(x => x.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeader = orderHeader.Where(x => x.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }
            return Json(new { data = orderHeader });         
		}
	}
}
