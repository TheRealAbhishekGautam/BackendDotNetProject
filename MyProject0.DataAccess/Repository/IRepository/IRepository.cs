using System;
using System.Linq.Expressions;

namespace MyProject0.DataAccess.Repository.IRepository
{
	public interface IRepository<T> where T : class
	{
		// T - Catagory

		// GetAll function for getting all of the catagories from the DB.

		IEnumerable<T> GetAll(string? IncludeProperties = null);

		// For getting a specific Catagary basis on some parameter.
		// Expression is basically referrs to Lambda Expressions, whenever we want to get the data
		// On the basis of a Lambda Expresson, this is what you need to add in interface.

		T Get(Expression<Func<T, bool>> filter, string? IncludeProperties = null);

		void Add (T entity);
		void Remove(T entity);
		void RemoveRange(IEnumerable<T> entity);

	}
}

