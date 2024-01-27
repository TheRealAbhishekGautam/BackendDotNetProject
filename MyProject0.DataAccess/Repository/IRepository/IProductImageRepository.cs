using System;
using MyProject0.Models;

namespace MyProject0.DataAccess.Repository.IRepository
{
	public interface IProductImageRepository : IRepository<ProductImage>
	{
		void Update(ProductImage obj);
	}
}

