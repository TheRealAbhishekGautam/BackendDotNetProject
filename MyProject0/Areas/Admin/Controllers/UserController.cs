using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyProject0.DataAccess.Data;
using MyProject0.DataAccess.Repository.IRepository;
using MyProject0.Models;
using MyProject0.Models.ViewModels;
using MyProject0.Utility;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyProject0.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        // We are directly using DbContext here.
        // The reason is we are going to access some Identity tables
        // Which are not present in the UnitOfWork class.
        // To access Identity tables from the DB just use the table name without AspNet
        // Example on the DB we have a table named :- AspNetUsers
        // If I want to access it on the code side I will access it using :- dbContext.Users.ToList();
        internal readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll ()
        {
            List<ApplicationUser> UsersList = _db.ApplicationUsers.Include(x => x.Company).ToList();

            var UserRoles = _db.UserRoles.ToList();
            var Roles = _db.Roles.ToList();

            foreach(var User in UsersList)
            {
                // Populating roles for all the users
                var RoleId = UserRoles.FirstOrDefault(x => x.UserId == User.Id).RoleId;
                User.Role = Roles.FirstOrDefault(x => x.Id == RoleId).Name;
                
                // If Company is null in that case just add an empty string so that js will not throw a warning.
                if (User.Company == null)
                {
                    User.Company = new Company() { Name = String.Empty };
                }
            }

            return Json(new { data = UsersList });
        }
        [HttpPost]
        public IActionResult LockUnlockAccount ([FromBody]string UserId)
        {
            var UserFromDb = _db.ApplicationUsers.FirstOrDefault(x => x.Id == UserId);
            if (UserFromDb == null)
            {
                return Json(new { success = false, message = "Error while Lock/Unlock the Account" });
            }

            if(UserFromDb.LockoutEnd != null && UserFromDb.LockoutEnd > DateTime.Now)
            {
                // The user is currently Locked, we need to Unlock it
                UserFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                // We need to Lock the user account
                UserFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            
            _db.SaveChanges();
            return Json(new { success = true, message = "Locked/Unlocked Successful" });
        }

        #endregion
    }
}
