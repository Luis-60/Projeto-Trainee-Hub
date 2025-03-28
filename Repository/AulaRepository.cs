using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Projeto_Trainee_Hub.Models;
namespace Projeto_Trainee_Hub.Repository
{

    public class AulaRepository 
    {
        private readonly MasterContext _context;
        public AulaRepository(MasterContext context)
        {
            _context = context;
        }

        public async Task<List<Aula>> GetAllAsync()
        {
            return await _context.Aulas.ToListAsync();
        }

        public async Task<Aula?> ObterPorIdCriadorAsync(int id)
        {
            return await _context.Aulas
                .Include(m => m.IdDocumentoNavigation)
                .FirstOrDefaultAsync(m => m.IdAula == id);
        }

        public async Task<IEnumerable<Aula>> FindAsync(Expression<Func<Aula, bool>> predicate)
        {
            return await _context.Aulas.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(Aula entity)
        {
            await _context.Aulas.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Aula entity)
        {
            _context.Aulas.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Aulas.FindAsync(id);
            if (entity != null)
            {
                _context.Aulas.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
