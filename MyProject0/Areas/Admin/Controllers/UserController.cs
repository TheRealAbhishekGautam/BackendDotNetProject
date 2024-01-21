using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RoleManagement (string UserId)
        {
            ApplicationUser UserFromDb = _db.ApplicationUsers.Where(x => x.Id == UserId).Include("Company").FirstOrDefault();

            var RoleListFromDb = _db.Roles.ToList();
            var CompanyListFromDb = _db.Companies.ToList();

            var CurrentRoleId = _db.UserRoles.FirstOrDefault(x => x.UserId == UserFromDb.Id).RoleId;
            var CurrentRole = RoleListFromDb.FirstOrDefault(x => x.Id == CurrentRoleId).Name;

            RoleManagementVM roleManagementVM = new()
            {
                MyUser = UserFromDb,
                RoleList = RoleListFromDb.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Name
                }),
                CompanyList = CompanyListFromDb.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            roleManagementVM.MyUser.Role = CurrentRole;

            return View(roleManagementVM);
        }

        [HttpPost]
        [ActionName("RoleManagement")]
        public IActionResult UpdateUser(RoleManagementVM roleManagementVM)
        {
            ApplicationUser OldUserFromDb = _db.ApplicationUsers.Where(x => x.Id == roleManagementVM.MyUser.Id).Include("Company").FirstOrDefault();

            var OldRoleId = _db.UserRoles.FirstOrDefault(x => x.UserId == OldUserFromDb.Id).RoleId;
            var OldRole = _db.Roles.FirstOrDefault(x => x.Id == OldRoleId).Name;

            // If old and new role is Company User but the company is changed

            if(roleManagementVM.MyUser.Role == SD.Role_Company 
                && OldRole == SD.Role_Company 
                && OldUserFromDb.CompanyId != roleManagementVM.MyUser.CompanyId)
            {
                OldUserFromDb.CompanyId = roleManagementVM.MyUser.CompanyId;
            }

            // If role is changed, update the role

            if(OldRole != roleManagementVM.MyUser.Role)
            {
                _userManager.RemoveFromRoleAsync(OldUserFromDb, OldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(OldUserFromDb, roleManagementVM.MyUser.Role).GetAwaiter().GetResult();

                // If new role is not compnay, Company Id should be null

                if (roleManagementVM.MyUser.Role != SD.Role_Company)
                {
                    OldUserFromDb.CompanyId = null;
                }

                // If new role is compnay, Update the Company Id

                if (roleManagementVM.MyUser.Role == SD.Role_Company)
                {
                    OldUserFromDb.CompanyId = roleManagementVM.MyUser.CompanyId;
                }
            }

            _db.SaveChanges();
            TempData["Success"] = "User Updated Successfully";
            return RedirectToAction(nameof(Index));
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
