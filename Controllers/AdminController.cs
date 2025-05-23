using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;
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
    private readonly AulaRepository _aulaRepository;  

    private readonly MasterContext _context;
    public AdminController(UsuariosRepository usuariosRepository, ModuloRepository moduloRepository, TreinamentoRepository treinamentoRepository, ISessao sessao, MasterContext context, AulaRepository aulaRepository)
    {
        _usuariosRepository = usuariosRepository;
        _treinamentoRepository = treinamentoRepository;
        _sessao = sessao;
        _moduloRepository = moduloRepository;
        _context = context;
        _aulaRepository = aulaRepository;
    }
    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> PerfilAsync()
    {
        var usuario = _sessao.BuscarSessaoUsuario();
        if (usuario == null)
        {
            return RedirectToAction("Login", "Home");
        }

        var idUsuario = usuario.IdUsuarios;
        var usuarios = await _usuariosRepository.GetByIdAsync(idUsuario);
        var usuarioTreinamentos = await _treinamentoRepository.GetByIdCriadorAsync(idUsuario);

        var progressoPorTreinamento = new Dictionary<int, int>();

        foreach (var treinamento in usuarioTreinamentos)
        {
            var modulos = _context.Modulos
                .Where(m => m.IdTreinamento == treinamento.IdTreinamentos)
                .Select(m => m.IdModulos)
                .ToList();

            var aulasIds = _context.Aulas
                .Where(a => modulos.Contains(a.IdModulo))
                .Select(a => a.IdAula)
                .ToList();

            var totalAulas = aulasIds.Count;
            var concluidas = _context.ProgressoAulas
                .Count(p => aulasIds.Contains(p.IdAula) && p.IdUsuario == idUsuario);

            int progresso = (totalAulas == 0) ? 0 : (int)Math.Round((double)concluidas * 100 / totalAulas);
            progressoPorTreinamento[treinamento.IdTreinamentos] = progresso;
        }

        var treinamentoUsuarios = new TreinamentoUsuariosViewModel
        {
            treinamentos = new Treinamento(),
            usuarios = usuarios,
            listaTreinamentos = usuarioTreinamentos,
            ProgressoPorTreinamento = progressoPorTreinamento
        };

        return View(treinamentoUsuarios);
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

        // Obtem todos os módulos e extrai os IDs
        var idsModulos = modulosTreinamento.Select(m => m.IdModulos).ToList();

        // Buscar todas as aulas desses módulos
        var aulasModulos = await _aulaRepository.GetByIdModuloListAsync(idsModulos);

        var treinamentoModulo = new TreinamentoModuloViewModel
        {
        treinamentos = treinamento,
        usuarios = usuario,
        listaModulos = modulosTreinamento,
        listaAulas = aulasModulos
        };
        return View(treinamentoModulo);
    }
    
        public IActionResult Modulos(int id)
    {
        var modulo = _context.Modulos.FirstOrDefault(m => m.IdModulos == id);

        if (modulo == null)
        {
            return NotFound();
        }

        var aulasDoModulo = _context.Aulas
                                    .Where(a => a.IdModulo == id)
                                    .ToList();

        var viewModel = new AulaModuloDocViewModel
        {
            modulos = modulo,
            listaAulas = aulasDoModulo,
            aulas = new Aula(), // Para o formul�rio do modal
            documentos = new Documento() // Para o formul�rio do modal
        };

        return View(viewModel);
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
