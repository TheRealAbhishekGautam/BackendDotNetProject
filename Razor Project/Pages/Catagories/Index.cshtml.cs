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
	public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public List<Catagory> CatagoryList;

        public IndexModel (ApplicationDbContext db)
        {
            _db = db;
        }

        // This is the get function and we are returning nothing from here to the veiw, since
        // All of the varibales are directly accessable to the view form here.
        public void OnGet()
        { 
            CatagoryList = _db.Catagories.ToList();
        }
    }
}
