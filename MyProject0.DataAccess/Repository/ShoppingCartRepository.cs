using System;
using MyProject0.DataAccess.Data;
using MyProject0.DataAccess.Repository.IRepository;
using MyProject0.Models;

namespace MyProject0.DataAccess.Repository
{
	public class ShoppingCartRepository : Repository <ShoppingCart> , IShoppingCartRepository
	{
        public readonly ApplicationDbContext _db;
		public ShoppingCartRepository(ApplicationDbContext db) : base(db)
		{
            _db = db;
		}

        public void Update(ShoppingCart obj)
        {
            _db.ShoppingCarts.Update(obj);
        }
    }
}

