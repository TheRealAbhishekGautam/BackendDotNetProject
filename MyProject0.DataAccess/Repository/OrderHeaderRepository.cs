using System;
using MyProject0.DataAccess.Data;
using MyProject0.DataAccess.Repository.IRepository;
using MyProject0.Models;

namespace MyProject0.DataAccess.Repository
{
	public class OrderHeaderRepository : Repository <OrderHeader> , IOrderHeaderRepository
	{
        public readonly ApplicationDbContext _db;
		public OrderHeaderRepository(ApplicationDbContext db) : base(db)
		{
            _db = db;
		}

        public void Update(OrderHeader obj)
        {
            _db.OrderHeaders.Update(obj);
        }
    }
}

