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
    public class ModuloRepository : BaseRepository<Modulo>
    {
        private readonly MasterContext _context;
        public ModuloRepository(MasterContext context) : base (context) 
        {
            _context = context;
        }

        public async Task<IEnumerable<Modulo?>> GetByIdTreinamentoAsync(int id)
        {
            return await _context.Modulos
                .Include(m => m.IdTreinamentoNavigation)
                .Where(m => m.IdTreinamento == id)
                .ToListAsync();
        }
        public async Task<Modulo?> GetByIdAsync(int id)
        {
            return await _context.Modulos
                .Include(m => m.IdTreinamentoNavigation)
                .FirstOrDefaultAsync(m => m.IdModulos == id);
        }
    }
}
