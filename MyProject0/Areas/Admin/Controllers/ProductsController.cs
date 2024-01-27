using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyProject0.DataAccess.Data;
using MyProject0.DataAccess.Repository.IRepository;
using MyProject0.Models;
using MyProject0.Models.ViewModels;
using MyProject0.Utility;
using System.Collections.Generic;

namespace MyProject0.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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
                obj.Product = _UnitOfWork.Product.Get(x => x.Id == id, "ProductImages"); 
                return View(obj);
            }

        }
        [HttpPost]
        public IActionResult Upsert (ProductsVM obj, List<IFormFile>? files)
        {
            if (ModelState.IsValid)
            {
                // Creating Case
                if (obj.Product.Id == 0)
                {
                    _UnitOfWork.Product.Add(obj.Product);
                }
                // Updating Case
                else
                {
                    _UnitOfWork.Product.Update(obj.Product);
                }

                // First we are creating the product then we will add the images corrosponding to that Product 
                // inside the folder that will be named after the Product Id which will be generated after we post a product.
                _UnitOfWork.Save();

                // WebRootPath will give us the path of the wwwroot folder.
                string wwwRootPath = _WebHostEnvironment.WebRootPath;

                if (files != null)
                {
                    foreach(var file in files)
                    {
                        string FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string ProductImagesPath = @"images\productimagefolders\product-" + obj.Product.Id;
                        string ProductImagesFinalPath = Path.Combine(wwwRootPath, ProductImagesPath);

                        // If the directory is not created yet then we are first creating it
                        if (!Directory.Exists(ProductImagesFinalPath))
                        {
                            Directory.CreateDirectory(ProductImagesFinalPath);
                        }

                        using (var FileStream = new FileStream(Path.Combine(ProductImagesFinalPath, FileName), FileMode.Create))
                        {
                            file.CopyTo(FileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ProductId = obj.Product.Id,
                            ImageUrl = @"/" + ProductImagesPath + @"/" + FileName
                        };

                        // Instanciating the ProductImges so that we can add images into it.
                        if(obj.Product.ProductImages == null)
                        {
                            obj.Product.ProductImages = new List<ProductImage>();
                        }

                        obj.Product.ProductImages.Add(productImage);

                    }

                    _UnitOfWork.Product.Update(obj.Product);
                    _UnitOfWork.Save();

                }
                
                TempData["Success"] = "The Product has been added/updated successfully";
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

        public IActionResult DeleteImage (int imageId)
        {
            var ImageToBeDeleted = _UnitOfWork.ProductImage.Get(x => x.Id == imageId);
            var ProductId = ImageToBeDeleted.ProductId;

            if (ImageToBeDeleted != null)
            {
                if(!String.IsNullOrEmpty(ImageToBeDeleted.ImageUrl))
                {
                    var OldImagePath = Path.Combine(_WebHostEnvironment.WebRootPath, ImageToBeDeleted.ImageUrl.TrimStart('/'));

                    if (OldImagePath != null && System.IO.File.Exists(OldImagePath))
                    {
                        System.IO.File.Delete(OldImagePath);
                    }
                }

                _UnitOfWork.ProductImage.Remove(ImageToBeDeleted); 
                _UnitOfWork.Save();

                TempData["Success"] = "Image Deleted Successfully";
            }

            return RedirectToAction(nameof(Upsert), new {id = ProductId });

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

            string ProductImagesPath = @"images\productimagefolders\product-" + id;
            string ProductImagesFinalPath = Path.Combine(_WebHostEnvironment.WebRootPath, ProductImagesPath);

            if (Directory.Exists(ProductImagesFinalPath))
            {
                // Fetching all the image names inside the directory
                string[] InsideFilePaths = Directory.GetFiles(ProductImagesFinalPath);

                // Deleting all the images indivisually
                foreach (string File in InsideFilePaths)
                {
                    System.IO.File.Delete(File);
                }

                // Finally deleting the empty directory
                Directory.Delete(ProductImagesFinalPath);
            }

            _UnitOfWork.Product.Remove(ProductToDelete); 
            _UnitOfWork.Save();
            return Json(new { success = true, message = "Deleted Successful" });
            // We don't want any view to show at the time of deleting just add a notification that the Product is deleted Successfully.
        }

        #endregion
    }
}
