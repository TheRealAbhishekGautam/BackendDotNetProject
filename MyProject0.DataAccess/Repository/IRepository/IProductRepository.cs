using MyProject0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject0.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product> 
    {
        public void Update (Product obj);

    }
}
