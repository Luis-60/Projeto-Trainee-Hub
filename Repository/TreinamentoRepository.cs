using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Projeto_Trainee_Hub.Models;
namespace Projeto_Trainee_Hub.Repository
{
    
    public class TreinamentoRepository : BaseRepository<Treinamento>
    {
        private readonly MasterContext _context;
        public TreinamentoRepository(MasterContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<Treinamento?> GetByIdAsync(int id)
        {
            return await _context.Treinamentos
                .Include(t => t.IdCriadorNavigation)
                .Include(t => t.IdEmpresaNavigation)
                .FirstOrDefaultAsync(t => t.IdTreinamentos == id);
        }
        public async Task<Treinamento?> GetByIdNoTrackingAsync(int id)
        {
            return await _context.Treinamentos.AsNoTracking()
                .FirstOrDefaultAsync(t => t.IdTreinamentos == id);
        }

        public async Task <IEnumerable<Treinamento?>> GetByIdCriadorAsync(int id)
        
        {
            return await _context.Treinamentos
                .Include(t => t.IdCriadorNavigation)
                .Include(t => t.IdEmpresaNavigation)
                .Include(t => t.IdCriadorNavigation.IdSetorNavigation)
                .Where(t => t.IdCriador == id)
                .ToListAsync();
        }
    }
}
