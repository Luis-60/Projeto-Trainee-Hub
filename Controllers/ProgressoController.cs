using Microsoft.AspNetCore.Mvc;
using Projeto_Trainee_Hub.Helper;
using Projeto_Trainee_Hub.Models;
using System.Linq;

namespace Projeto_Trainee_Hub.Controllers
{
    public class ProgressoController : Controller
    {
        private readonly MasterContext _context;
        private readonly ISessao _sessao;

        public ProgressoController(MasterContext context, ISessao sessao)
        {
            _context = context;
            _sessao = sessao;
        }

        [HttpPost]
        public IActionResult ConcluirAula(int idAula, int idUsuario)
        {
            var jaConcluida = _context.ProgressoAulas
                .Any(p => p.IdAula == idAula && p.IdUsuario == idUsuario);

            if (!jaConcluida)
            {
                var progresso = new ProgressoAula
                {
                    IdAula = idAula,
                    IdUsuario = idUsuario,
                    DataConclusao = DateTime.Now
                };

                _context.ProgressoAulas.Add(progresso);
                _context.SaveChanges();
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public IActionResult ProgressoTreinamento(int idModulo)
        {
            var usuario = _sessao.BuscarSessaoUsuario();
            if (usuario == null) return RedirectToAction("Login", "Home");

            var totalAulas = _context.Aulas.Count(a => a.IdModulo == idModulo);
            var aulasConcluidas = _context.ProgressoAulas
                .Count(p => p.Aula.IdModulo == idModulo && p.IdUsuario == usuario.IdUsuarios);

            var progresso = totalAulas == 0 ? 0 : (aulasConcluidas * 100 / totalAulas);
            ViewBag.Porcentagem = progresso;

            return View();
        }

    }
}