using System;
using MyProject0.Data;
using MyProject0.DataAccess.Repository.IRepository;
using MyProject0.Models;

namespace MyProject0.DataAccess.Repository
{
	public class CatagoryRepository : Repository <Catagory> , ICatagoryRepository
	{
        public readonly ApplicationDbContext _db;

        // Since Repository.cs is not called directky from anywhere and is consumed by other Repository files like this one,
        // from where it will get the value of ApplicationDbContext db .? and we have used db for doing a lot of things there.
        // So for that, we have done constructor chaining here, base constructor i.e. Repository class constructor will be
        // executed first before this constructor and the value of db is passed to it too.
		public CatagoryRepository (ApplicationDbContext db) : base(db)
		{
            _db = db;
		}

        public void Update(Catagory obj)
        {
            _db.Catagories.Update(obj);
        }
    }
}

