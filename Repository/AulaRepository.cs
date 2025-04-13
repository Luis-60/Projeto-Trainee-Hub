using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Interfaces;

namespace Projeto_Trainee_Hub.Repository
{
    public class AulaRepository : BaseRepository<Aula>
    {
        protected readonly MasterContext _context;
        public AulaRepository(MasterContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<Aula?> GetByIdAsync(int id)
        {
            return await _context.Aulas
                .Include(m => m.IdModuloNavigation)
                .FirstOrDefaultAsync(m => m.IdAula == id);
        }
    }
}