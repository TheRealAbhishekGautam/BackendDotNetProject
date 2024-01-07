using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyProject0.DataAccess.Data;
using MyProject0.DataAccess.DbInitializers.IDbInitializer;
using MyProject0.Models;
using MyProject0.Utility;

// On production how we are going to seed the first admin account, apply migrations and create roles.?
// Here on the dev env. we have done it using the Hacky way.
// But on Production we use DbInitializer to do it. That will do all the intial setup.

namespace MyProject0.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // Where we will invoke this method which will do all the initial setup on the production.?
        // Inside Program.cs file.

        public void Initialize()
        {
            // We have to do the following things here :- 
            // Migrations if they are not applied

            try
            {
                if(_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
            }

            // Create roles if not created

            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();

                // If roles are just created then we need to make the first admin user too.

                _userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = "admin@production.com",
                    Email = "admin@production.com",
                    Name = "Admin",
                    PhoneNumber = "1234567890",
                    StreetAddress = "56, New Delhi",
                    PostalCode = "12345",
                    State = "Delhi",
                    City = "New Delhi"
                }, "AdminPassword@123").GetAwaiter().GetResult();

                ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@production.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }

            return;

        }
    }
}
