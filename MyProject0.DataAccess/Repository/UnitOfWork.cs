﻿using System;
using MyProject0.Data;
using MyProject0.DataAccess.Repository.IRepository;

namespace MyProject0.DataAccess.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
        private readonly ApplicationDbContext _db;
        public ICatagoryRepository Catagory { get; set; }

        public UnitOfWork(ApplicationDbContext db)
		{
            _db = db;
            Catagory = new CatagoryRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
