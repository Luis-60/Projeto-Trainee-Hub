using Projeto_Trainee_Hub.Models;

namespace Projeto_Trainee_Hub.Repository
{
    public class TipoRepository 
    {
        private readonly MasterContext _context;

        public TipoRepository(MasterContext context)
        {
            _context = context;
        }

        public List<Tipo> BuscarTodos()
        {
            return _context.Tipos.ToList();
        }
    }
}
