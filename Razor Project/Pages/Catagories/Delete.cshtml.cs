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
    [BindProperties]
	public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public Catagory? Catagory { get; set; }

        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult OnGet(int? ID)
        {
            if (ID == 0 || ID == null)
            {
                return NotFound();
            }

            Catagory = _db.Catagories.Find(ID);

            if (Catagory == null)
            {
                return NotFound();
            }

            return Page();
        }
        public IActionResult OnPost()
        {
            if (Catagory == null)
            {
                return NotFound();
            }

            _db.Catagories.Remove(Catagory);
            _db.SaveChanges();
            TempData["Error"] = "The Record has been deleted successfully";
            return RedirectToPage("Index");
        }
        
    }
}
