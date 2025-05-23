using Microsoft.EntityFrameworkCore;
using Projeto_Trainee_Hub.Models;

namespace Projeto_Trainee_Hub.Repository
{
    public class EmpresaRepository : BaseRepository<Empresa>
    {
        private readonly MasterContext _context;

        public EmpresaRepository(MasterContext context) : base(context)
        {
            _context = context;
        }

        // Buscar empresa por ID
        public async Task<Empresa?> GetByIdAsync(int id)
        {
            return await _context.Empresas
                .FirstOrDefaultAsync(e => e.IdEmpresa == id);
        }

        // Listar todas as empresas
        public async Task<List<Empresa>> GetAllAsync()
        {
            return await _context.Empresas.ToListAsync();
        }

        // Atualizar empresa
        public async Task AtualizarAsync(Empresa empresa)
        {
            _context.Empresas.Update(empresa);
            await _context.SaveChangesAsync();
        }

        // Excluir empresa
        public async Task ExcluirAsync(int id)
        {
            var empresa = await _context.Empresas.FirstOrDefaultAsync(e => e.IdEmpresa == id);
            if (empresa != null)
            {
                _context.Empresas.Remove(empresa);
                await _context.SaveChangesAsync();
            }
        }
    }
}
