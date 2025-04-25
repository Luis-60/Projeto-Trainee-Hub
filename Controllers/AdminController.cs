using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;
using AspNetCoreGeneratedDocument;
using Microsoft.EntityFrameworkCore;
using Projeto_Trainee_Hub.ViewModel;
using Projeto_Trainee_Hub.Helper;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = "2")]
public class AdminController : Controller
{
    private readonly UsuariosRepository _usuariosRepository;
    private readonly ISessao _sessao;
    private readonly TreinamentoRepository _treinamentoRepository;
    private readonly ModuloRepository _moduloRepository;

    private readonly MasterContext _context;
    public AdminController(UsuariosRepository usuariosRepository, ModuloRepository moduloRepository, TreinamentoRepository treinamentoRepository, ISessao sessao, MasterContext context)
    {
        _usuariosRepository = usuariosRepository;
        _treinamentoRepository = treinamentoRepository;
        _sessao = sessao;
        _moduloRepository = moduloRepository;
        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> PerfilAsync()
    {
        
        var usuario = _sessao.BuscarSessaoUsuario();
        if (usuario == null)
        {
            return View();
        }
        
        var idUsuario = usuario.IdUsuarios;
        if (idUsuario != null)
        {
            var usuarios = await _usuariosRepository.GetByIdAsync(idUsuario);
            var usuarioTreinamentos = await _treinamentoRepository.GetByIdCriadorAsync(idUsuario);
            var treinamentoUsuarios = new TreinamentoUsuariosViewModel{treinamentos = new Treinamento(), usuarios = usuarios, listaTreinamentos = usuarioTreinamentos};

        return View(treinamentoUsuarios);
        }
        
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> TreinamentosAsync(int id)
    {
        var usuario = _sessao.BuscarSessaoUsuario();
        if (usuario == null)
        {
            return RedirectToAction("Login", "Home");
        }
        var treinamento = await _treinamentoRepository.GetByIdAsync(id);
        if (treinamento == null)
        {
            return NotFound(); 
        }
        
        var modulosTreinamento = await _moduloRepository.GetByIdTreinamentoAsync(treinamento.IdTreinamentos);

        var treinamentoModulo = new TreinamentoModuloViewModel{treinamentos = treinamento, usuarios = usuario, listaModulos = modulosTreinamento};
        return View(treinamentoModulo);
    }

    public async Task<IActionResult> ModulosAsync(int id)
    {
        var usuario = _sessao.BuscarSessaoUsuario();
        if (usuario == null)
        {
            return RedirectToAction("Login", "Home");
        }
        var treinamento = await _treinamentoRepository.GetByIdAsync(id);
        if (treinamento == null)
        {
            return NotFound(); 
        }
            var treinamentoUsuarios = new TreinamentoUsuariosViewModel{treinamentos = treinamento, usuarios = usuario};
        return View(treinamentoUsuarios);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
        public IActionResult CriarUsuarios()
    {
        var viewModel = new UsuarioSetorTipoViewModel
        {
            Tipos = _usuariosRepository.GetTodosTipos()
                .Select(t => new SelectListItem { Value = t.IdTipo.ToString(), Text = t.Nome }).ToList(),

            Setores = _usuariosRepository.GetTodosSetores()
                .Select(s => new SelectListItem { Value = s.IdSetor.ToString(), Text = s.Nome }).ToList()
        };

        return View(viewModel);
    }

    public IActionResult GerirUsuarios()
    {
        var usuarioLogado = _sessao.BuscarSessaoUsuario();


        int idEmpresa = (int)usuarioLogado.IdEmpresa;

        var usuarios = _context.Usuarios
            .Include(u => u.IdSetorNavigation)
            .Include(u => u.IdTipoNavigation)
            .Where(u => u.IdEmpresa == idEmpresa) // << Filtro aqui
            .Select(u => new UsuarioSetorTipoViewModel
            {
                UsuarioId = u.IdUsuarios,
                Nome = u.Nome,
                Email = u.Email,
                Matricula = u.Matricula,
                Senha = u.Senha,
                Idade = u.Idade,
                Setor = u.IdSetorNavigation != null ? u.IdSetorNavigation.Nome : "",
                Tipo = u.IdTipoNavigation != null ? u.IdTipoNavigation.Nome : ""
            })
            .ToList();

        return View(usuarios);
    }


    [HttpPost]
    public async Task<IActionResult> CriarUsuarios(UsuarioSetorTipoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Tipos = _usuariosRepository.GetTodosTipos()
                .Select(t => new SelectListItem { Value = t.IdTipo.ToString(), Text = t.Nome }).ToList();

            model.Setores = _usuariosRepository.GetTodosSetores()
                .Select(s => new SelectListItem { Value = s.IdSetor.ToString(), Text = s.Nome }).ToList();

            
        }

        
        var usuarioLogado = _sessao.BuscarSessaoUsuario();
        if (usuarioLogado == null)
        {
            return RedirectToAction("Login", "Home");
        }

        
        var novoUsuario = new Usuarios
        {
            Nome = model.Nome,
            Email = model.Email,
            Matricula = model.Matricula,
            Senha = model.Senha,
            Idade = model.Idade,
            IdSetor = model.IdSetor,
            IdTipo = model.IdTipo,
            IdEmpresa = usuarioLogado.IdEmpresa 
        };

       await _usuariosRepository.AddAsync(novoUsuario);

        return RedirectToAction("GerirUsuarios", "Admin");
    }
    public List<SelectListItem> ObterSetores(int? idSetorSelecionado)
    {
        return _context.Setors
            .Select(s => new SelectListItem
            {
                Value = s.IdSetor.ToString(),
                Text = s.Nome,
                Selected = s.IdSetor == idSetorSelecionado
            }).ToList();
    }
    public List<SelectListItem> ObterTipos(int? idSetorSelecionado)
    {
        return _context.Tipos
            .Select(t => new SelectListItem
            {
                Value = t.IdTipo.ToString(),
                Text = t.Nome,
                Selected = t.IdTipo == idSetorSelecionado
            }).ToList();
    }

    [HttpGet]
    public IActionResult AcoesUsuarios(int id)
    {
        
        var usuario = _usuariosRepository.ObterUsuarioPorId(id);

        
        var viewModel = new UsuarioSetorTipoViewModel
        {
            UsuarioId = usuario.IdUsuarios,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Matricula = usuario.Matricula,
            Senha = usuario.Senha,
            Idade = usuario.Idade,
            IdSetor = usuario.IdSetor,
            IdTipo = usuario.IdTipo,
            Setores = ObterSetores(id),
            Tipos = ObterTipos(id)
        };

        return View(viewModel);
    }


    [HttpPost]
    public async Task<IActionResult> EditarUsuario(UsuarioSetorTipoViewModel viewModel)
    {


        var usuarioExistente = _context.Usuarios
            .Include(u => u.IdSetorNavigation)
            .Include(u => u.IdTipoNavigation)
            .FirstOrDefault(u => u.IdUsuarios == viewModel.UsuarioId);

            usuarioExistente.Nome = viewModel.Nome;
            usuarioExistente.Email = viewModel.Email;
            usuarioExistente.Matricula = viewModel.Matricula;
            usuarioExistente.Senha = viewModel.Senha;
            usuarioExistente.Idade = viewModel.Idade;
            usuarioExistente.Imagem = usuarioExistente.Imagem;
            usuarioExistente.QtdTreinamentos = usuarioExistente.QtdTreinamentos;
            usuarioExistente.IdSetor = viewModel.IdSetor;
            usuarioExistente.IdTipo = viewModel.IdTipo;
            usuarioExistente.IdEmpresa = usuarioExistente.IdEmpresa;

               
            await _usuariosRepository.UpdateAsync(usuarioExistente);

            return RedirectToAction("GerirUsuarios", "Admin"); 
            
    }
    [HttpPost]
    public async Task<IActionResult> ExcluirUsuario(int id)
    {
        await _usuariosRepository.DeleteAsync(id);
        return RedirectToAction("GerirUsuarios", "Admin");
    }
}
