using System;
using MyProject0.Models;

namespace MyProject0.DataAccess.Repository.IRepository
{
	public interface IOrderHeaderRepository : IRepository<OrderHeader>
	{
		void Update(OrderHeader obj);
	}
}

