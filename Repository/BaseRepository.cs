using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Interfaces;

namespace Projeto_Trainee_Hub.Repository
{
    public class BaseRepository<T> : IBaseRepository<T>, IDisposable where T : class
    {
        private readonly MasterContext _context;
        private readonly DbSet<T> _dbSet;
        public BaseRepository(MasterContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
