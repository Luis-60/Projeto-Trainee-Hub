using Microsoft.AspNetCore.Mvc;
using Projeto_Trainee_Hub.Helper;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.ViewModel;
using System.IO;
using System.Linq;
using static System.Net.WebRequestMethods;

namespace Projeto_Trainee_Hub.Controllers
{
    public class AulasController : Controller
    {
        private readonly MasterContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadFolder;
        private readonly ISessao _sessao;


        public AulasController(MasterContext context, IWebHostEnvironment environment, ISessao sessao)
        {
            _context = context;
            _environment = environment;
            _sessao = sessao;
            _uploadFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(_uploadFolder))
            {
                Directory.CreateDirectory(_uploadFolder);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Criar(string Nome, string Descricao, int IdModulo, string VideoUrl, List<IFormFile> files)
        {
            if (string.IsNullOrWhiteSpace(Nome) || IdModulo == 0)
            {
                Console.WriteLine("Dados inválidos.");
                return RedirectToAction("Modulo", "Modulos", new { id = IdModulo });
            }

            var aula = new Aula
            {
                Nome = Nome,
                Descricao = Descricao,
                IdModulo = IdModulo,
                VideoUrl = VideoUrl
            };

            _context.Aulas.Add(aula);
            await _context.SaveChangesAsync();

            if (files != null && files.Any())
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var caminho = Path.Combine(_uploadFolder, file.FileName);
                        using (var stream = new FileStream(caminho, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var documento = new Documento
                        {
                            Nome = file.FileName,
                            IdAula = aula.IdAula
                        };

                        _context.Documentos.Add(documento);
                    }
                }
                await _context.SaveChangesAsync();
            }

            if (Request.Headers.ContainsKey("Referer"))
                return Redirect(Request.Headers["Referer"].ToString());

            return RedirectToAction("Modulos", "AulasController");
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Aula aula, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                var aulaBanco = _context.Aulas.FirstOrDefault(x => x.IdAula == aula.IdAula);

                if (aulaBanco == null)
                    return NotFound();

                aulaBanco.Nome = aula.Nome;
                aulaBanco.Descricao = aula.Descricao;
                aulaBanco.IdModulo = aula.IdModulo;
                aulaBanco.VideoUrl = aula.VideoUrl;


                var docsAntigos = _context.Documentos.Where(d => d.IdAula == aula.IdAula).ToList();
                foreach (var doc in docsAntigos)
                {
                    var caminhoFisico = Path.Combine(_uploadFolder, Path.GetFileName(doc.Nome));
                    if (System.IO.File.Exists(caminhoFisico))
                        System.IO.File.Delete(caminhoFisico);

                    _context.Documentos.Remove(doc);
                }

                _context.SaveChanges();

                if (files != null && files.Any())
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            var nomeArquivo = Path.GetFileName(file.FileName);
                            var filePath = Path.Combine(_uploadFolder, nomeArquivo);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            var documento = new Documento
                            {
                                Nome = nomeArquivo,
                                IdAula = aulaBanco.IdAula
                            };

                            _context.Documentos.Add(documento);
                        }
                    }

                    _context.SaveChanges();
                    await _context.SaveChangesAsync();
                }

                if (Request.Headers.ContainsKey("Referer"))
                    return Redirect(Request.Headers["Referer"].ToString());
                else
                    return RedirectToAction("Modulos", "AulasController");
            }

            if (Request.Headers.ContainsKey("Referer"))
                return Redirect(Request.Headers["Referer"].ToString());
            else
                return RedirectToAction("Modulos", "AulasController");
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            var aula = await _context.Aulas.FindAsync(id);
            if (aula != null)
            {
                var documentos = _context.Documentos.Where(d => d.IdAula == aula.IdAula).ToList();

                foreach (var doc in documentos)
                {
                    var caminhoFisico = Path.Combine(_uploadFolder, doc.Nome);
                    if (System.IO.File.Exists(caminhoFisico))
                        System.IO.File.Delete(caminhoFisico);

                    _context.Documentos.Remove(doc);
                }

                _context.Aulas.Remove(aula);
                await _context.SaveChangesAsync();
            }

            if (Request.Headers.ContainsKey("Referer"))
                return Redirect(Request.Headers["Referer"].ToString());
            else
                return RedirectToAction("Modulos", "AulasController");
        }
        [HttpPost]
        public IActionResult ConcluirAula(int idAula)
        {
            // Recuperar o usuário logado da sessão
            var usuario = _sessao.BuscarSessaoUsuario(); // ou outro método que você esteja usando
            if (usuario == null)
            {
                return Unauthorized();
            }

            int idUsuario = usuario.IdUsuarios;

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

            if (Request.Headers.ContainsKey("Referer"))
                return Redirect(Request.Headers["Referer"].ToString());

            return RedirectToAction("Modulos", "Aulas");
        }

    }
}