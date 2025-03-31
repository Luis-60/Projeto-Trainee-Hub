using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Projeto_Trainee_Hub.Models;
namespace Projeto_Trainee_Hub.Repository
{

    public class ModuloRepository 
    {
        private readonly MasterContext _context;
        public ModuloRepository(MasterContext context)
        {
            _context = context;
        }

        public async Task<List<Modulo>> GetAllAsync()
        {
            return await _context.Modulos.ToListAsync();
        }

        public async Task<Modulo?> ObterPorIdCriadorAsync(int id)
        {
            return await _context.Modulos
                .Include(m => m.IdTreinamentoNavigation)
                .FirstOrDefaultAsync(m => m.IdModulos == id);
        }

        public async Task<IEnumerable<Modulo>> FindAsync(Expression<Func<Modulo, bool>> predicate)
        {
            return await _context.Modulos.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(Modulo entity)
        {
            await _context.Modulos.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Modulo entity)
        {
            _context.Modulos.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Modulos.FindAsync(id);
            if (entity != null)
            {
                _context.Modulos.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
