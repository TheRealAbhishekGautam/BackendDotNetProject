using System;
using MyProject0.DataAccess.Data;
using MyProject0.DataAccess.Repository.IRepository;
using MyProject0.Models;

namespace MyProject0.DataAccess.Repository
{
	public class OrderDetailRepository : Repository <OrderDetail> , IOrderDetailRepository
	{
        public readonly ApplicationDbContext _db;
		public OrderDetailRepository(ApplicationDbContext db) : base(db)
		{
            _db = db;
		}

        public void Update(OrderDetail obj)
        {
            _db.OrderDetails.Update(obj);
        }
    }
}

