﻿using System;
namespace MyProject0.DataAccess.Repository.IRepository
{
	public interface IUnitOfWork
	{
		public ICatagoryRepository Catagory { get; }
        public IProductRepository Product { get; }
		public ICompanyRepository Company { get; }
        public IShoppingCartRepository ShoppingCart { get; }
        public IApplicationUserRepository ApplicationUser { get; }
        public IOrderHeaderRepository OrderHeader { get; }
        public IOrderDetailRepository OrderDetail { get; }
        void Save();
	}
}

