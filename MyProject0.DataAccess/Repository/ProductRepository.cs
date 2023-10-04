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
            _db.Product.Update(obj);
        }
    }
}
