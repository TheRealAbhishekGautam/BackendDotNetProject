using System;
using MyProject0.Models;

namespace MyProject0.DataAccess.Repository.IRepository
{
	public interface IOrderDetailRepository : IRepository<OrderDetail>
	{
		void Update(OrderDetail obj);
	}
}

