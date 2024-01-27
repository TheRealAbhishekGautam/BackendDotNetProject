using System;
using MyProject0.DataAccess.Data;
using MyProject0.DataAccess.Repository.IRepository;
using MyProject0.Models;

namespace MyProject0.DataAccess.Repository
{
	public class ProductImageRepository : Repository <ProductImage> , IProductImageRepository
	{
        public readonly ApplicationDbContext _db;
		public ProductImageRepository (ApplicationDbContext db) : base(db)
		{
            _db = db;
		}

        public void Update(ProductImage obj)
        {
            _db.ProductImages.Update(obj);
        }
    }
}

