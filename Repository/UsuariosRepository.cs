using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Projeto_Trainee_Hub.Models;

namespace Projeto_Trainee_Hub.Repository
{

    public class UsuariosRepository : BaseRepository<Usuarios>
    {
        private readonly MasterContext _context;

        public UsuariosRepository(MasterContext context) : base(context)
        {
            _context = context;
        }


        public async Task<Usuarios?> GetByMatriculaAsync(string matricula)
        {
            return await _context.Usuarios
            .Include(u => u.IdSetorNavigation)
            .Include(u => u.IdTipoNavigation)
            .FirstOrDefaultAsync(u => u.Matricula == matricula);
        }

        public async Task<Usuarios?> GetByIdAsync(int id)
        {
            return await _context.Usuarios
            .Include(u => u.IdSetorNavigation)
            .Include(u => u.IdTipoNavigation)
            .Include(u => u.IdEmpresaNavigation)
            .FirstOrDefaultAsync(u => u.IdUsuarios == id);
        }
        public async Task<string> GetMatriculaByIdAsync(int id)
        {
            var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.IdUsuarios == id);
            return usuario.Matricula;
        }

        
        public Usuarios? ValidateUser(string matricula, string senha)
        {
            return _context.Usuarios
                .FirstOrDefault(u => u.Matricula == matricula && u.Senha == senha);
        }

        public Usuarios? GetUserLoggedIn(HttpContext httpContext)
        {
            throw new NotImplementedException();
        }
    }
}
