using System;
using MyProject0.DataAccess.Data;
using MyProject0.DataAccess.Repository.IRepository;

namespace MyProject0.DataAccess.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
        private readonly ApplicationDbContext _db;
        public ICatagoryRepository Catagory { get; set; }
        public IProductRepository Product { get; set; }

        public UnitOfWork(ApplicationDbContext db)
		{
            _db = db;
            Catagory = new CatagoryRepository(_db);
            Product = new ProductRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}

