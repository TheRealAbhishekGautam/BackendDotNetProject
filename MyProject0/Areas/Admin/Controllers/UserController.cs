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
        // To access Identity tables from the DB just use the table name without AspNet
        // Example on the DB we have a table named :- AspNetUsers
        // If I want to access it on the code side I will access it using :- dbContext.Users.ToList();
        
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        public UserController(UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RoleManagement (string UserId)
        {
            RoleManagementVM roleManagementVM = new()
            {
                MyUser = _unitOfWork.ApplicationUser.Get(x => x.Id == UserId, "Company"),
                RoleList = _roleManager.Roles.Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Name
                }),
                CompanyList = _unitOfWork.Company.GetAll().Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            roleManagementVM.MyUser.Role = _userManager.GetRolesAsync(roleManagementVM.MyUser).GetAwaiter().GetResult().FirstOrDefault();

            return View(roleManagementVM);
        }

        [HttpPost]
        [ActionName("RoleManagement")]
        public IActionResult UpdateUser(RoleManagementVM roleManagementVM)
        {
            ApplicationUser OldUserFromDb = _unitOfWork.ApplicationUser.Get(x => x.Id == roleManagementVM.MyUser.Id, "Company", true);

            var OldRole = _userManager.GetRolesAsync(OldUserFromDb).GetAwaiter().GetResult().FirstOrDefault();

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

            // Since we are fetching the OldUserFromDb without AsNotracking,
            // It will automatically make all the changes to the db.

            // _unitOfWork.ApplicationUser.Update(OldUserFromDb);
            _unitOfWork.Save();
            TempData["Success"] = "User Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll ()
        {
            List<ApplicationUser> UsersList = _unitOfWork.ApplicationUser.GetAll(IncludeProperties:"Company").ToList();

            foreach(var User in UsersList)
            {
                // Populating roles for all the users
                User.Role = _userManager.GetRolesAsync(User).GetAwaiter().GetResult().FirstOrDefault();
                
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
            var UserFromDb = _unitOfWork.ApplicationUser.Get(x => x.Id == UserId);
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

            _unitOfWork.ApplicationUser.Update(UserFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Locked/Unlocked Successful" });
        }

        #endregion
    }
}
