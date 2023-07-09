using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor_Project.Data;
using Razor_Project.Models;

namespace Razor_Project.Pages.Catagories
{
	public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        // In razor pages we have to explecitly bind the properties if we need to post
        // the catagory to the database. We can write [BindProperty] above every property or
        // we can add [BindProperties] above the class CreateModel and all of the properties
        // will be binded.
        [BindProperty]
        public Catagory Catagory { get; set; }

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {

        }
        public IActionResult OnPost()
        {
            _db.Catagories.Add(Catagory);
            _db.SaveChanges();
            TempData["Success"] = "The Record has been added successfully";
            return RedirectToPage("Index");
        }
    }
}
