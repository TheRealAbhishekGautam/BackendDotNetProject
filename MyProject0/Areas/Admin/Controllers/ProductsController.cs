using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyProject0.DataAccess.Data;
using MyProject0.DataAccess.Repository.IRepository;
using MyProject0.Models;
using MyProject0.Models.ViewModels;
using System.Collections.Generic;

namespace MyProject0.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        internal readonly IUnitOfWork _UnitOfWork;
        public ProductsController(IUnitOfWork unitofwork)
        {
            _UnitOfWork = unitofwork;
        }
        public IActionResult Index()
        {
            List<Product> productsList = _UnitOfWork.Product.GetAll().ToList();
            return View(productsList);
        }
        // Upsert is a combination of Update and Insert i.e. we are doing both of the things inside the same view.
        // So we will have two API's one for returning the respective fields in which we can create a new product or update an existing product
        // And another one in which we are making an post call.
        // If the id that has been passes here is null then it's insert otherwise it will be update.
        // For both of them we will have the same view. (Advantage of doing this)
        public IActionResult Upsert (int? id)
        {
            IEnumerable<SelectListItem> MyCatagoryList = _UnitOfWork.Catagory.GetAll()
                                                       .Select(x => new SelectListItem()
                                                       {
                                                           Text = x.Name,
                                                           Value = x.ID.ToString()
                                                       });
            ProductsVM obj = new ProductsVM()
            {
                CatagoryList = MyCatagoryList,
                Product = new Product()
            };
            // Insert case
            if(id == null || id == 0)
            {
                // Just return the regular view of insert.
                return View(obj);
            }
            else
            {
                obj.Product = _UnitOfWork.Product.Get(x => x.Id == id); 
                return View(obj);
            }

        }
        [HttpPost]
        public IActionResult Upsert (ProductsVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                _UnitOfWork.Product.Add(obj.Product);
                _UnitOfWork.Save();
                TempData["Success"] = "The Product has been added successfully";
                return RedirectToAction("Index");
            }
            else
            {
                // We are sending this list again because old one will be gone.
                obj.CatagoryList = _UnitOfWork.Catagory.GetAll().Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.ID.ToString()
                });
                return View(obj);
            }
        }
        // We have combined both of the funcitonality in the same Upsert API's so we don't need these anymore.
        /*
        public IActionResult Create()
        {
            // This is called Projections in .Net, It's a very powerful thing since we are actually converting
            // an object of one type to another and only considering the required properties.
            IEnumerable<SelectListItem> MyCatagoryList = _UnitOfWork.Catagory.GetAll()
                                                       .Select(x => new SelectListItem()
                                                       {
                                                           Text = x.Name,
                                                           Value = x.ID.ToString()
                                                       });
            // In the return statement we can only return one thing at a time to the View,
            // What if we want to return more than one thing or multiple views to the view.?
            // In that requirement we use ViewBag/ViewData/ViewModels.

            // ViewBag.CatagoryList = CatagoryList;
            // ViewData["CatagoryList"] = CatagoryList;
            ProductsVM obj = new ProductsVM() {
                CatagoryList = MyCatagoryList,
                Product = new Product()
            };
            return View(obj);
        }
        [HttpPost]
        public IActionResult Create(ProductsVM obj)
        {
            if (ModelState.IsValid)
            {
                _UnitOfWork.Product.Add(obj.Product);
                _UnitOfWork.Save();
                TempData["Success"] = "The Product has been added successfully";
                return RedirectToAction("Index");
            }
            else{
                // We are sending this list again because old one will be gone.
                obj.CatagoryList = _UnitOfWork.Catagory.GetAll().Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.ID.ToString()
                });
                return View(obj);
            }
        }
        public IActionResult Edit(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            Product? productfromDB = _UnitOfWork.Product.Get(x => x.Id == id);

            if (productfromDB == null)
            {
                return NotFound();
            }

            return View(productfromDB);
        }

        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _UnitOfWork.Product.Update(obj);
                _UnitOfWork.Save();
                TempData["Success"] = "The Record has been modified successfully";
                return RedirectToAction("Index");
            }

            return View();

        }
        */
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }

            Product? productfromDB = _UnitOfWork.Product.Get(x => x.Id == id);

            if (productfromDB == null)
            {
                return NotFound();
            }

            return View(productfromDB);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product? obj = _UnitOfWork.Product.Get(x => x.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            _UnitOfWork.Product.Remove(obj);
            _UnitOfWork.Save();
            TempData["Success"] = "The Product has been deleted successfully";
            return RedirectToAction("Index");

        }
    }
}
