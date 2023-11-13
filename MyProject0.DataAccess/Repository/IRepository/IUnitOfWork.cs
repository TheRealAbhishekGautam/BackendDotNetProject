using System;
namespace MyProject0.DataAccess.Repository.IRepository
{
	public interface IUnitOfWork
	{
		public ICatagoryRepository Catagory { get; set; }
        public IProductRepository Product { get; set; }
		public ICompanyRepository Company { get; set; }
        void Save();
	}
}

