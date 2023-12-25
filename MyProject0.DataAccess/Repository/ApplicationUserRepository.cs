﻿using System;
using MyProject0.DataAccess.Data;
using MyProject0.DataAccess.Repository.IRepository;
using MyProject0.Models;

namespace MyProject0.DataAccess.Repository
{
	public class ApplicationUserRepository : Repository <ApplicationUser> , IApplicationUserRepository
	{
        public readonly ApplicationDbContext _db;
		public ApplicationUserRepository(ApplicationDbContext db) : base(db)
		{
            _db = db;
		}
    }
}

