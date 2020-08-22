using NanoShop.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NanoShop.Core.Repositories.Products
{
    public class ProductRepository : GenericRepository<Entities.Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbContext)
       : base(dbContext)
        {
        }
    }
}
