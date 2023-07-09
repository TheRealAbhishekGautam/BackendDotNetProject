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
	public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public Catagory? Catagory { get; set; }

        public EditModel (ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet (int? id)
        {
            if (id != 0 && id != null)
            {
                Catagory = _db.Catagories.Find(id);
            }
        }
        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                _db.Catagories.Update(Catagory);
                _db.SaveChanges();
                TempData["Success"] = "The Record has been Edited successfully";
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
