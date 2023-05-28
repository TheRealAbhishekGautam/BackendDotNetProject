using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProject0.Data;
using MyProject0.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyProject0.Controllers
{
    public class CatagoryController : Controller
    {
        // Here we are assining all the values from our ApplicationDbContext to a local variable.
        private readonly ApplicationDbContext _db;
        public CatagoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        public IActionResult Index()
        {
            List <Catagory> objCatagoryList = _db.Catagories.ToList();
            return View(objCatagoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        // We can have action methods of same name but their type should be different.
        [HttpPost]
        public IActionResult Create (Catagory obj)
        {
            // First we are checking that weather the values that has been returned by the view/user are passing
            // our Server side validations or not.

            if (ModelState.IsValid)
            {
                // Now since we get the values of the object that needs to be added inside the database,
                // we will use ef core to add those values to the db.
                _db.Add(obj);
                // We can add multiple objects then save it once.
                _db.SaveChanges();
                // Now after adding the values inside the db, we have to refresh code side too for seeing the updated values.
                // For that we are using a function RedirectToAction,
                // Using this we can redirect to any of the action within any of the controller.
                // RedirectToAction("ActionMethodName","ControllerName)
                // But if we are calling an action method of the same controller, then we don't need to add the controller name.
                return RedirectToAction("Index");
            }

            // Now if we are not able to add the values insde the database then we should be on the same page only and not
            // Redirected to the page which shows all the updated values.
            // We also want to display the error the same page only.

            return View();
            
        }
    }
}

