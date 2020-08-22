using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NanoShop.Core.Common
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        Task<TEntity> GetById(int id);
        Task<TEntity> Create(TEntity entity);
        Task<TEntity> Update(TEntity entity);
        Task Delete(int id);
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> where);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> where);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> where);

        //Task BulkInsert(List<TEntity> list);
        //Task BulkUpdate(List<TEntity> list);
        //Task BulkDelete(List<TEntity> list);
    }
}
