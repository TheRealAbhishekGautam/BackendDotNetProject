using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProject0.DataAccess.Repository.IRepository;
using MyProject0.Models;
using MyProject0.Models.ViewModels;
using MyProject0.Utility;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace MyProject0.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        internal readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new ShoppingCartVM() {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x=> x.ApplicationUserId == userId,"Product"),
                OrderHeader = new()
            };

            IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

            foreach(var cart in ShoppingCartVM.ShoppingCartList)
            {
                // Adding all the images of the Product.
                cart.Product.ProductImages = productImages.Where(x => x.ProductId == cart.Product.Id).ToList();

                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Count * cart.Price);
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;


            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Count * cart.Price);
            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
		public IActionResult SummaryPost()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, "Product");
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

			// Don't do like this
			// ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);
            // When you will save the values inside the OrderHeader it will also Save the value of ApplicationUser again 
            // And when it will try to save it, it will throw an exception saying that this UserId already exists.
            // So never populate the navigational property of an entity from the db if it already exists.

			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);

			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Count * cart.Price);
			}

            // If for the current user threre's a Company Id associated inside the database, we directly needs to place the order 
            // Rather then redirecting it to a payment gateway.
            if(applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
				// It's a regular customer account.
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
				ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPeding;
            }
            else
            {
				// Company User Account
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
				ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
			}

            // Now Let's Save the Order Header
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            // Now Let's Save the Order Detail
            // We have to save all the products to the db inside the OrderDetail table.
            foreach(var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Count = cart.Count,
                    Price = cart.Price
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            // We are capturing the payment of a regular customer account order.
			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				// Stripe Logic
                // To create a session 
                // All of this code is according to the stripe documentation.
				StripeConfiguration.ApiKey = "sk_test_51OSztTSENbH4Zz5foEptEWuS325DAyGE7HexyWzW829nGWTG7okFRQ527HVGPerjXJhltpZEuSYT6gX4dSM4jvSA00zXbUqXmP";

                // We can't give base url as localhost staticly since on the production this url will be different
                // var baseUrl = "https://localhost:44381/";

                // So we are making it dynamicly to pick the url on which the site is running currently.
                var baseUrl = Request.Scheme + "://" + Request.Host.Value + "/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = baseUrl + $"Customer/Cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = baseUrl + "Customer/Cart/Index",
					// Inside the LineItems we have to give the list of all products and quantity of the order.
                    // We are adding it below.
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
				};

                foreach(var items in  ShoppingCartVM.ShoppingCartList)
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
                // Here we are creating a sesion to make a payment
				Session session = service.Create(options);
                // PayementIntentId will be null here since we are just creating a session currently
                _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();

                // This is the url where we will be redirected (To the stripe website) from here.
                Response.Headers.Add("Location", session.Url);

                // Status Code 303 means we are redirect to an url.
                return new StatusCodeResult(303);
			}

			return RedirectToAction(nameof(OrderConfirmation), new {id = ShoppingCartVM.OrderHeader.Id});
		}

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == id,"ApplicationUser");
            
            if(orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                // Case of a Regular Customer order.
                // Here we are checking weather customer completed the payment from Stripe or not.
                // Just to update the PaymentIntentId, PaymentStatus and OrderStatus in our Db.
                // Inside Session Service we have an api to get that whole session.
                // First we are creating an object of SessionService.

                var service = new SessionService();

                var session = service.Get(orderHeader.SessionId);

                if(session.PaymentStatus.ToLower() == "paid")
                {
					_unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
					_unitOfWork.Save();
				}
            }

            // Making the shopping cart empty after successfully placing the order.

            List<ShoppingCart> shoppingCart = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCart);
            _unitOfWork.Save();

            // Ending the session after order confirmation
            HttpContext.Session.Clear();

            return View(id);
        }

		public IActionResult Plus(int cartId)
        {
            ShoppingCart ShoppingCartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId);
            ShoppingCartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(ShoppingCartFromDb);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        public IActionResult Minus (int cartId)
        {
            ShoppingCart ShoppingCartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId, track:true);
            if (ShoppingCartFromDb.Count == 1)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == ShoppingCartFromDb.ApplicationUserId).Count()-1);
                _unitOfWork.ShoppingCart.Remove(ShoppingCartFromDb);
            }
            else
            {
                ShoppingCartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(ShoppingCartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Remove (int cartId)
        {
            ShoppingCart ShoppingCartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId, track:true);
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == ShoppingCartFromDb.ApplicationUserId).Count() - 1);
            _unitOfWork.ShoppingCart.Remove(ShoppingCartFromDb);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if(shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else if(shoppingCart.Count <= 100)
            {
                return shoppingCart.Product.Price50;
            }
            else
            {
                return shoppingCart.Product.Price100;
            }
        }
    }
}
