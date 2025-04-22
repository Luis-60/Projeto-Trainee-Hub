using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;
using AspNetCoreGeneratedDocument;
using Projeto_Trainee_Hub.ViewModel;
using Projeto_Trainee_Hub.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = "2")]


public class AdminController : Controller
{
    private readonly UsuariosRepository _usuariosRepository;
    private readonly ISessao _sessao;
    private readonly TreinamentoRepository _treinamentoRepository;
    private readonly ModuloRepository _moduloRepository;
    private readonly MasterContext _context;
    
    public AdminController(UsuariosRepository usuariosRepository, ModuloRepository moduloRepository,TreinamentoRepository treinamentoRepository, ISessao sessao, MasterContext context)
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
    public IActionResult GerirUsuarios() 
    {
        return View();
    
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


    [HttpGet]
    public async Task<IActionResult> GetUsuarios()
    {
        var usuarios = await _context.Usuarios
            .Include(u => u.IdSetorNavigation)
            .Include(u => u.IdTipoNavigation)
            .Include(u => u.IdEmpresaNavigation)
            .Select(u => new {
                u.IdUsuarios,
                u.Nome,
                u.Email,
                u.Matricula,
                u.Senha,
                u.Idade,
                Setor = u.IdSetorNavigation != null ? u.IdSetorNavigation.Nome : "",
                Tipo = u.IdTipoNavigation != null ? u.IdTipoNavigation.Nome : "",
                Empresa = u.IdEmpresaNavigation != null ? u.IdEmpresaNavigation.Nome : ""
            })
            .ToListAsync();

        return Json(new { data = usuarios });
    }

    [HttpPost]
    public IActionResult CriarUsuarios(UsuarioSetorTipoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Tipos = _usuariosRepository.GetTodosTipos()
                .Select(t => new SelectListItem { Value = t.IdTipo.ToString(), Text = t.Nome }).ToList();

            model.Setores = _usuariosRepository.GetTodosSetores()
                .Select(s => new SelectListItem { Value = s.IdSetor.ToString(), Text = s.Nome }).ToList();

            
        }

        // Pega o usuário logado da sessão
        var usuarioLogado = _sessao.BuscarSessaoUsuario();
        if (usuarioLogado == null)
        {
            return RedirectToAction("Login", "Home");
        }

        // Cria novo objeto de Usuarios com os dados da ViewModel
        var novoUsuario = new Usuarios
        {
            Nome = model.Nome,
            Email = model.Email,
            Matricula = model.Matricula,
            Senha = model.Senha,
            Idade = model.Idade,
            IdSetor = model.IdSetor,
            IdTipo = model.IdTipo,
            IdEmpresa = usuarioLogado.IdEmpresa // pega da sessão, não do input!
        };

        _usuariosRepository.AddAsync(novoUsuario);

        return RedirectToAction("GerirUsuarios", "Admin");
    }




}
