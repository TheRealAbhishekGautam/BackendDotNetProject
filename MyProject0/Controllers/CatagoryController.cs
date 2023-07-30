using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProject0.Data;
using MyProject0.DataAccess.Repository.IRepository;
using MyProject0.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyProject0.Controllers
{
    public class CatagoryController : Controller
    {
        // Here we are assining all the values from our ApplicationDbContext to a local variable.
        private readonly IUnitOfWork _UnitOfWork;
        public CatagoryController(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }
        
        public IActionResult Index()
        {
            List <Catagory> objCatagoryList = _UnitOfWork.Catagory.GetAll().ToList();
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

            // Now if we want some custom validations then we will add them here.
            // Here we are adding a custom validation that name and display order can't be same.
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Name and Display Order must have different values");
            }

            // Here we are adding an error message but it doesnot corrosponds to any of the feild/key, so where it will be printed.?
            // It will only be printed inside the asp-validation-summary = "All/ModelOnly" and not with any of the fields.
            if (obj.Name.ToLower() == "test")
            {
                ModelState.AddModelError("", "Item name test is not allowed");
            }

            if (ModelState.IsValid)
            {
                // Now since we get the values of the object that needs to be added inside the database,
                // we will use ef core to add those values to the db.
                _UnitOfWork.Catagory.Add(obj);
                // We can add multiple objects then save it once.
                _UnitOfWork.Save();
                // TempData is a pre-defined array, inside which the data will be available only till the next render.
                // We can add some confirmation messages inside this.
                // Success is the unique key that will be used to get this message inside the tempdata.
                // We don't even have to import this to the views, it's available there already.
                TempData["Success"] = "The Record has been added successfully";
                // Now after adding the values inside the db, we have to refresh code side too for seeing the updated values.
                // For that we are using a function RedirectToAction,
                // Using this we can redirect to any of the action within any of the controller.
                // RedirectToAction("ActionMethodName","ControllerName")
                // But if we are calling an action method of the same controller, then we don't need to add the controller name.
                return RedirectToAction("Index");
            }

            // Now if we are not able to add the values insde the database then we should be on the same page only and not
            // Redirected to the page which shows all the updated values.
            // We also want to display the error on the same page only.

            return View();
            
        }

        // If we are not specifying the type of aciton method, it is considered as "get" by default.

        public IActionResult Edit(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            Catagory? catagoryfromDB = _UnitOfWork.Catagory.Get(x => x.ID == id);

            // Find() will find the row with the help of the given primary key.
            //Catagory? catagoryfromDB = _db.Catagories.Find(id);

            // FirstorDefault will find weather the given recored exists or not, if not it will return a null object
            // It also works wiht the non-primary columns
            // Catagory? catagoryfromDB1 = _db.Catagories.FirstOrDefault(x => x.ID==id);

            // Where will return all of the values which matches with the given parameter.
            // It also works wiht the non-primary columns.
            // Catagory? catagoryfromDB2 = _db.Catagories.Where( x => x.ID == id).FirstOrDefault();


            if (catagoryfromDB == null)
            {
                return NotFound();
            }

            return View(catagoryfromDB);
        }

        [HttpPost]
        public IActionResult Edit(Catagory obj)
        {
            if (ModelState.IsValid)
            {
                _UnitOfWork.Catagory.Update(obj);
                _UnitOfWork.Save();
                TempData["Success"] = "The Record has been modified successfully";
                return RedirectToAction("Index");
            }

            return View();

        }

        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            Catagory? catagoryfromDB = _UnitOfWork.Catagory.Get(x => x.ID == id);

            if (catagoryfromDB == null)
            {
                return NotFound();
            }

            return View(catagoryfromDB);
        }

        // Since when we return the corrosponding data from the view, it will search for the
        // Same name action method inside this Controller however we can't have the same name
        // ActionMethods with the same parameteres, so we are changing it's name and telling the
        // compiler that its what you are finding inside the ActionName.

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost (int? id)
        {
            Catagory? obj = _UnitOfWork.Catagory.Get(x => x.ID == id);

            if (obj == null)
            {
                return NotFound();
            }

            _UnitOfWork.Catagory.Remove(obj);
            _UnitOfWork.Save();
            TempData["Success"] = "The Record has been deleted successfully";
            return RedirectToAction("Index");

        }

    }
}

