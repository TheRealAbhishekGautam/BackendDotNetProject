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
