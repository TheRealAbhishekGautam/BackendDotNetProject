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
        
        // This is the Interface which will be used to access the files inside wwwroot (The place where all of the static files are stored)
        internal readonly IWebHostEnvironment _WebHostEnvironment;
        public ProductsController(IUnitOfWork unitofwork, IWebHostEnvironment webHostEnvironment)
        {
            _UnitOfWork = unitofwork;
            _WebHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            // The parameter that we are passing in the GetAll funciton should be Sensitive since we are directly adding it in the LINQ
            // If it will not match with anything on the DB it will throw an error.
            List<Product> productsList = _UnitOfWork.Product.GetAll(IncludeProperties:"Catagory").ToList();
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
                // WebRootPath will give us the path of the wwwroot folder.
                string wwwRootPath = _WebHostEnvironment.WebRootPath;
                if (file != null)
                {
                    // Since the file that was uploaded by the user can be of any name and the user can add multiple images with the same name for multiple products
                    // So, We are changing it by a new guid + the actual extention given by the file from the user.
                    string FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // We are combining the path of the actual wwwroot folder with the path at which we are adding the images of products.
                    // Now the ProductPath is the actual path at which we are supposed to add the images of Products.
                    string ProductPath = Path.Combine(wwwRootPath, @"images/products");

                    // If we are updating a Product's image then we also need to delete the old image corrosponding to that product
                    if(!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        var OldImagePath = Path.Combine(wwwRootPath,obj.Product.ImageUrl.TrimStart('/'));

                        if(System.IO.File.Exists(OldImagePath))
                        {
                            System.IO.File.Delete(OldImagePath);
                        }
                    }

                    // Now we have the new name of the file and the location where we are suposed to save/add the file
                    // Let's add it
                    using (var FileStream = new FileStream (Path.Combine(ProductPath,FileName),FileMode.Create))
                    {
                        file.CopyTo(FileStream);
                    }
                    obj.Product.ImageUrl = @"/images/products/" + FileName;
                }
                
                // Creating Case
                if(obj.Product.Id == 0)
                {
                    _UnitOfWork.Product.Add(obj.Product);
                }
                // Updating Case
                else
                {
                    _UnitOfWork.Product.Update(obj.Product);
                }
                
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
        
        /*
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
        */
        #region API Calls

        [HttpGet]
        public IActionResult GetAll ()
        {
            List<Product> ProductsList = _UnitOfWork.Product.GetAll(IncludeProperties: "Catagory").ToList();
            return Json(new { data = ProductsList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Product ProductToDelete = _UnitOfWork.Product.Get(x => x.Id == id);
            if (ProductToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            // Remember whenever we are deleting the product we also need to delete it's corrosponding image 
            var OldImagePath = Path.Combine(_WebHostEnvironment.WebRootPath, ProductToDelete.ImageUrl.TrimStart('/'));

            if (System.IO.File.Exists(OldImagePath))
            {
                System.IO.File.Delete(OldImagePath);
            }
            _UnitOfWork.Product.Remove(ProductToDelete); 
            _UnitOfWork.Save();
            return Json(new { success = true, message = "Deleted Successful" });
            // We don't want any view to show at the time of deleting just add a notification that the Product is deleted Successfully.
        }

        #endregion
    }
}
