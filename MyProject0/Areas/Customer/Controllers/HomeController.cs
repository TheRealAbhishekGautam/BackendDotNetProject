using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyProject0.DataAccess.Repository;
using MyProject0.DataAccess.Repository.IRepository;
using MyProject0.Models;

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
        IEnumerable<Product> ProductList = _unitOfWork.Product.GetAll(IncludeProperties: "Catagory");
        // We have not mentioned what view from which locaion, then how the corrosponding view is getting returned.?
        // It will go inside the view folder and search for the folder named with the same name as the controller and then
        // Search for the view with the same name as the ActionResult method name (Index in our case). 
        return View(ProductList);
        // Moreover we can also return a specific view, for example
        //return View("privacy");
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

