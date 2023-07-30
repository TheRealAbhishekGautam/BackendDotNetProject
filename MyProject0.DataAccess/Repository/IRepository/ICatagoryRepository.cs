using System;
using MyProject0.Models;

namespace MyProject0.DataAccess.Repository.IRepository
{
	public interface ICatagoryRepository : IRepository<Catagory>
	{
		void Update(Catagory obj);
	}
}

