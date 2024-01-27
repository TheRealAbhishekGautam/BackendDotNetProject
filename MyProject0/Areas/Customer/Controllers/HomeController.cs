using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProject0.DataAccess.Repository;
using MyProject0.DataAccess.Repository.IRepository;
using MyProject0.Models;
using MyProject0.Utility;

namespace MyProject0.Areas.Customers.Controllers;

// Telling the controller that you corrosponds to which area.
[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitofwork)
    {
        _logger = logger;
        _unitOfWork = unitofwork;
    }

    public IActionResult Index()
    {        
        IEnumerable<Product> ProductList = _unitOfWork.Product.GetAll(IncludeProperties: "Catagory,ProductImages");
        // We have not mentioned what view from which locaion, then how the corrosponding view is getting returned.?
        // It will go inside the view folder and search for the folder named with the same name as the controller and then
        // Search for the view with the same name as the ActionResult method name (Index in our case). 
        return View(ProductList);
        // Moreover we can also return a specific view, for example
        //return View("privacy");
    }
    public IActionResult Details(int ProductId)
    {
        ShoppingCart ShoppingCart = new ShoppingCart()
        {
            Product = _unitOfWork.Product.Get(x => x.Id == ProductId, IncludeProperties: "Catagory,ProductImages"),
            Count = 1,
            ProductId = ProductId
        };
        return View(ShoppingCart);
    }
    // We are adding the authrize in order to get the details of the user
    // If the user is not logged in, it will ask them to login first
    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCart ShopCart)
    {
        // Now in order to get the details of the user we are using ClaimsIdentity

        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShopCart.ApplicationUserId = userId;

        // Here we are checking if a cart with the same user and product is there inside the database or not.
        // If it's there then edit the count of the product else add it.
        ShoppingCart ShoppingCartFromDb = _unitOfWork.ShoppingCart.Get(x => x.ApplicationUserId == userId && x.ProductId == ShopCart.ProductId);

        if( ShoppingCartFromDb != null )
        {
            // Edit Case
            ShoppingCartFromDb.Count += ShopCart.Count;
            _unitOfWork.ShoppingCart.Update(ShoppingCartFromDb);
            _unitOfWork.Save();
        }
        else
        {
            // Add Case
            _unitOfWork.ShoppingCart.Add(ShopCart);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId).Count());
        }

        TempData["Success"] = "Cart Updated Successfully";

        return RedirectToAction("Index");
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

