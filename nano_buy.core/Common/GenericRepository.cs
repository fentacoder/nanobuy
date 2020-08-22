using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NanoShop.Core.Common
{
    public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly ApplicationDbContext _dbContext;
        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IQueryable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>().AsNoTracking();
        }

        public async Task<TEntity> GetById(int id)
        {
            return await _dbContext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }
        public async Task<TEntity> Create(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            await SaveChanges();
            return entity;
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            await SaveChanges();
            return entity;
        }

        public async Task Delete(int id)
        {
            var entity = await GetById(id);
            _dbContext.Set<TEntity>().Remove(entity);
            await SaveChanges();
        }

        //public async Task BulkInsert(List<TEntity> list)
        //{
        //    await _dbContext.Set<TEntity>().BulkInsertAsync(list);
        //}

        //public async Task BulkUpdate(List<TEntity> list)
        //{
        //    await _dbContext.Set<TEntity>().BulkUpdateAsync(list);
        //}

        //public async Task BulkDelete(List<TEntity> list)
        //{
        //    await _dbContext.Set<TEntity>().BulkDeleteAsync(list);
        //}

        public async Task SaveChanges()
        {
            var entries = _dbContext.ChangeTracker.Entries().Where(e => e.Entity is AuditableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

            foreach (var entityEntry in entries)
            {

                if (entityEntry.State == EntityState.Added)
                    ((AuditableEntity)entityEntry.Entity).CreatedDateTime = DateTime.Now;

                if (entityEntry.State == EntityState.Modified)
                    ((AuditableEntity)entityEntry.Entity).UpdatedDateTime = DateTime.Now;

                if (entityEntry.State == EntityState.Deleted)
                    ((AuditableEntity)entityEntry.Entity).DeletedDateTime = DateTime.Now;
            }
            await _dbContext.SaveChangesAsync();
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> where)
        {
            return _dbContext.Set<TEntity>().Where(where);
        }
        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> where)
        {
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(where);
        }
        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> where)
        {
            return _dbContext.Set<TEntity>().FirstOrDefault(where);
        }
    }
}
