using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyProject0.DataAccess.Data;
using MyProject0.DataAccess.Repository.IRepository;
using Newtonsoft.Json.Linq;

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
            _db.Product.Include(x => x.Catagory).Include(x => x.CatagoryId);
        }

        public void Add(T entity)
        {
            // Since we are currently dealing with generics, We cant do like
            // _db.Catagories.Add(abc) OR _db.T.Add();
            // We have to declare an internal variable inside this class and
            // Inititlize it's value to the required value inside the Constructor.

            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? IncludeProperties = null)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            if (!string.IsNullOrEmpty(IncludeProperties))
            {
                foreach (var i in IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    // We are making a kind of Generic LINQ, whatsoever parameteres we are getting we are directly
                    // adding it to the query by Include.
                    query = query.Include(i);
                }
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(string? IncludeProperties = null)
        {
            IQueryable<T> values = dbSet;
            if (!string.IsNullOrEmpty(IncludeProperties))
            {
                foreach(var i in IncludeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    values = values.Include(i);
                }
            }
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

