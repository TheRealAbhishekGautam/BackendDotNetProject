using System;
using MyProject0.Models;

namespace MyProject0.DataAccess.Repository.IRepository
{
	public interface IOrderHeaderRepository : IRepository<OrderHeader>
	{
		void Update(OrderHeader obj);
		void UpdateStatus(int Id, string OrderStatus, string? PaymentStatus = null);
		void UpdateStripePaymentId(int Id, string SessinId, string PaymentIntentId);
	}
}

