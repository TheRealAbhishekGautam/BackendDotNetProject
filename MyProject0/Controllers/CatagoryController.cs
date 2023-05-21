using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProject0.Data;
using MyProject0.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyProject0.Controllers
{
    public class CatagoryController : Controller
    {
        // Here we are assining all the values from our ApplicationDbContext to a local variable.
        private readonly ApplicationDbContext _db;
        public CatagoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        public IActionResult Index()
        {
            List <Catagory> objCatagoryList = _db.Catagories.ToList();
            return View(objCatagoryList);
        }
    }
}

