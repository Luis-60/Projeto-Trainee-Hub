using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Projeto_Trainee_Hub.Models;
namespace Projeto_Trainee_Hub.Repository
{

    public class UsuariosTreinamentosRepository : BaseRepository<UsuariosTreinamento>
    {
        private readonly MasterContext _context;
        public UsuariosTreinamentosRepository(MasterContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UsuariosTreinamento?>> ObterTreinamentosEmAndamento()
        {
            DateTime hoje = DateTime.Today;

            return _context.UsuariosTreinamentos
                .Where(ut => ut.DataInicio <= hoje && ut.DataTermino >= hoje && ut.Progresso > 0 && ut.Progresso < 100)
                .ToList();
        }
    }
}