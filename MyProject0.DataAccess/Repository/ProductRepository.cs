using MyProject0.DataAccess.Data;
using MyProject0.DataAccess.Repository.IRepository;
using MyProject0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject0.DataAccess.Repository
{
    internal class ProductRepository : Repository<Product>, IProductRepository
    {
        internal readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }
        public void Update (Product obj) { 
            // This is the update by ef core
            _db.Product.Update(obj);

            // We can also do our custom updation
            // In which we can add our logic to update too.
            var ProductFromDb = _db.Product.FirstOrDefault(x=> x.Id == obj.Id);
            if (ProductFromDb != null)
            {
                ProductFromDb.Title = obj.Title;
                ProductFromDb.ISBN = obj.ISBN;
                ProductFromDb.Price = obj.Price;
                ProductFromDb.Price50 = obj.Price50;
                ProductFromDb.Price100 = obj.Price100;
                ProductFromDb.ListPrice = obj.ListPrice;
                ProductFromDb.Description = obj.Description;
                ProductFromDb.CatagoryId = obj.CatagoryId;
                ProductFromDb.Author = obj.Author;

                if(ProductFromDb.ImageUrl != null) {
                    ProductFromDb.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}
