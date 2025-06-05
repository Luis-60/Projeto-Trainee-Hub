using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projeto_Trainee_Hub.Helper;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;
using Projeto_Trainee_Hub.ViewModel;

namespace Projeto_Trainee_Hub.Controllers
{
    public class AdminController : Controller
    {
        private readonly UsuariosRepository _usuariosRepository;
        private readonly ISessao _sessao;
        private readonly TreinamentoRepository _treinamentoRepository;
        private readonly ModuloRepository _moduloRepository;
        private readonly AulaRepository _aulaRepository;
        private readonly EmpresaRepository _empresaRepository;

        private readonly MasterContext _context;
        public AdminController(UsuariosRepository usuariosRepository, ModuloRepository moduloRepository, TreinamentoRepository treinamentoRepository, ISessao sessao, MasterContext context, EmpresaRepository empresaRepository)
        {
            _usuariosRepository = usuariosRepository;
            _treinamentoRepository = treinamentoRepository;
            _sessao = sessao;
            _moduloRepository = moduloRepository;
            _context = context;
            _empresaRepository = empresaRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Admin()
        {
            var usuario = _sessao.BuscarSessaoUsuario();
            if (usuario == null)
            {
                return View(); // exibe a view "Admin.cshtml"
            }

            return RedirectToAction("Administracao"); // redireciona para a action Administracao()
        }

        public async Task<IActionResult> Administracao()
        {
            var empresas = await _context.Empresas.ToListAsync();

            var viewModel = empresas.Select(e => new EmpresaUsuarioSetorTipoViewModel
            {
                IdEmpresa = e.IdEmpresa,
                NomeEmpresa = e.Nome,
                Descricao = e.Descricao,
                EmailEmpresa = e.Email,
                Telefone = e.Telefone,
                Codigo = e.Codigo
            }).ToList();

            return View(viewModel);

        }
        [HttpGet]
        public async Task<IActionResult> AcoesEmpresas(int id)
        {
            var empresa = await _empresaRepository.GetByIdAsync(id);

            if (empresa == null)
                return NotFound();

            var viewModel = new EmpresaUsuarioSetorTipoViewModel
            {
                IdEmpresa = empresa.IdEmpresa,
                NomeEmpresa = empresa.Nome,
                Descricao = empresa.Descricao,
                EmailEmpresa = empresa.Email,
                Telefone = empresa.Telefone,
                Codigo = empresa.Codigo
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> EditarEmpresa(EmpresaUsuarioSetorTipoViewModel viewModel)
        {


            var EmpresaExistente = _context.Empresas
                .FirstOrDefault(e => e.IdEmpresa == viewModel.IdEmpresa);

            EmpresaExistente.Nome = viewModel.NomeEmpresa;
            EmpresaExistente.Descricao = viewModel.Descricao;
            EmpresaExistente.Email = viewModel.EmailEmpresa;
            EmpresaExistente.Telefone = viewModel.Telefone;
            EmpresaExistente.Codigo = viewModel.Codigo;

            await _empresaRepository.UpdateAsync(EmpresaExistente);

            return RedirectToAction("Administracao", "Admin");

        }
        [HttpPost]
        public async Task<IActionResult> ExcluirEmpresa(int id)
        {
            await _empresaRepository.DeleteAsync(id);
            return RedirectToAction("Administracao", "Admin");
        } 
    [HttpPost]
        public async Task<IActionResult> CriarEmpresas(EmpresaUsuarioSetorTipoViewModel model)
        {

            var usuarioLogado = _sessao.BuscarSessaoUsuario();
            if (usuarioLogado == null)
            {
                return RedirectToAction("Login", "Home");
            }


            var novaEmpresa = new Empresa
            {
                Nome = model.NomeEmpresa,
                Descricao = model.Descricao,
                Email = model.EmailEmpresa,
                Telefone = model.Telefone,
                Codigo = model.Codigo
            };
            

            await _empresaRepository.AddAsync(novaEmpresa);

            return RedirectToAction("Administracao", "Admin");
        }
        [HttpGet]
        public IActionResult CriarEmpresa()
        {
            
            return View();
        }

    }
}