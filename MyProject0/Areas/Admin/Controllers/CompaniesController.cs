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
    public class CompaniesController : Controller
    {
        internal readonly IUnitOfWork _UnitOfWork;
        public CompaniesController(IUnitOfWork unitofwork)
        {
            _UnitOfWork = unitofwork;
        }
        public IActionResult Index()
        {
            List<Company> CompaniesList = _UnitOfWork.Company.GetAll().ToList();
            return View(CompaniesList);
        }

        public IActionResult Upsert (int? id)
        {
            // Insert case
            if(id == null || id == 0)
            {
                // Just return the regular view of insert.
                return View(new Company());
            }
            // Update Case
            else
            {
                Company obj = _UnitOfWork.Company.Get(x => x.Id == id); 
                return View(obj);
            }

        }
        [HttpPost]
        public IActionResult Upsert (Company obj)
        {
            if (ModelState.IsValid)
            {
                // Creating Case
                if(obj.Id == 0)
                {
                    _UnitOfWork.Company.Add(obj);
                    _UnitOfWork.Save();
                    TempData["Success"] = "The Compnay has been Added successfully";
                    return RedirectToAction("Index");
                }
                // Updating Case
                else
                {
                    _UnitOfWork.Company.Update(obj);
                    _UnitOfWork.Save();
                    TempData["Success"] = "The Compnay has been Updated successfully";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(obj);
            }
        }
        #region API Calls

        [HttpGet]
        public IActionResult GetAll ()
        {
            List<Company> CompaniesList = _UnitOfWork.Company.GetAll().ToList();
            return Json(new { data = CompaniesList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Company CompanyToDelete = _UnitOfWork.Company.Get(x => x.Id == id);
            if (CompanyToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _UnitOfWork.Company.Remove(CompanyToDelete); 
            _UnitOfWork.Save();
            return Json(new { success = true, message = "Deleted Successful" });
        }

        #endregion
    }
}
