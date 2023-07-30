using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyProject0.Data;
using MyProject0.DataAccess.Repository.IRepository;

namespace MyProject0.DataAccess.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

		public Repository(ApplicationDbContext db)
		{
            _db = db;
            // Now this is equivalent to : _db.Catagories == dbSet (This Internal Variable)
            this.dbSet = _db.Set<T>();
        }

        public void Add(T entity)
        {
            // Since we are currently dealing with generics, We cant do like
            // _db.Catagories.Add(abc) OR _db.T.Add();
            // We have to declare an internal variable inside this class and
            // Inititlize it's value to the required value inside the Constructor.

            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            IEnumerable<T> query = dbSet;
            // I was getting this error
            // cannot convert from 'System.Linq.Expressions.Expression<System.Func<T, bool>>' to 'System.Func<T, bool>'
            // So for resilving it we have to use .Compile() function.
            query = query.Where(filter.Compile());
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll()
        {
            IEnumerable<T> values = dbSet;
            return values.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            // RemoveRange is a inbuilt function inside EF core used to remove a range of values.
            // It expects a list of values that needs to be removed.
            dbSet.RemoveRange(entity);
        }
    }
}

