using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Projeto_Trainee_Hub.Models;
namespace Projeto_Trainee_Hub.Repository
{

    public class UsuariosRepository 
    {
        private readonly MasterContext _context;

        public UsuariosRepository(MasterContext context)
        {
            _context = context;
        }

        public async Task<List<Usuarios>> GetAllAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public Usuarios? ValidarUsuario(string matricula, string senha)
        {
            return _context.Usuarios
                .FirstOrDefault(u => u.Matricula == matricula && u.Senha == senha);
        }

        public async Task<IEnumerable<Usuarios>> FindAsync(Expression<Func<Usuarios, bool>> predicate)
        {
            return await _context.Usuarios.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(Usuarios entity)
        {
            await _context.Usuarios.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuarios entity)
        {
            _context.Usuarios.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Usuarios.FindAsync(id);
            if (entity != null)
            {
                _context.Usuarios.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
