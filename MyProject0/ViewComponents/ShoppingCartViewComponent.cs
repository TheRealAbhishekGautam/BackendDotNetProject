using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyProject0.DataAccess.Repository.IRepository;
using MyProject0.Utility;
using System.Security.Claims;

namespace MyProject0.ViewComponents
{
    // For making a view component we have to add ViewComponent in the last of a class name
    // Also need to inherit ViewComponent class in this.
    public class ShoppingCartViewComponent : ViewComponent
    {
        public IUnitOfWork _unitOfWork { get; set; }
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            if (claimsIdentity.Claims != null && claimsIdentity.Claims.Count() != 0)
            {
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                // If userId is null that means the user is not logged in
                // If not we need to retrive the cart count and display it
                if (userId != null && HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId).Count());
                }
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
