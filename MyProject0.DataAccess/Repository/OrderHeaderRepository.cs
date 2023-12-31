using System;
using Microsoft.IdentityModel.Tokens;
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

		public void UpdateStatus(int Id, string OrderStatus, string? PaymentStatus = null)
		{
			OrderHeader OrderHeaderFormDb = _db.OrderHeaders.FirstOrDefault(x => x.Id == Id);

			if(OrderHeaderFormDb != null)
			{
				OrderHeaderFormDb.OrderStatus = OrderStatus;

				if(!PaymentStatus.IsNullOrEmpty())
				{
					OrderHeaderFormDb.PaymentStatus = PaymentStatus;
				}
			}
		}

		public void UpdateStripePaymentId(int Id, string SessinId, string PaymentIntentId)
		{
			OrderHeader OrderHeaderFormDb = _db.OrderHeaders.FirstOrDefault(x => x.Id == Id);

			// SessinId is basically when a customer starts to make the payment
			// Remember we get time for making every online payment that we make
			// Payment Intent Id is only genetrated when the payment is completed inside the session.

			if(!SessinId.IsNullOrEmpty())
			{
				OrderHeaderFormDb.SessionId = SessinId;
			}
			if (!PaymentIntentId.IsNullOrEmpty())
			{
				OrderHeaderFormDb.PaymentIntentId = PaymentIntentId;
				OrderHeaderFormDb.OrderDate = DateTime.Now;
			}
		}
	}
}

