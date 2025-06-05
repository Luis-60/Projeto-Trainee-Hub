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

[Authorize(Roles = "4")]
public class EncarregadoController : Controller
{
    private readonly UsuariosRepository _usuariosRepository;
    private readonly ISessao _sessao;
    private readonly TreinamentoRepository _treinamentoRepository;
    private readonly ModuloRepository _moduloRepository;
    private readonly AulaRepository _aulaRepository;

    private readonly MasterContext _context;
    public EncarregadoController(UsuariosRepository usuariosRepository, ModuloRepository moduloRepository, TreinamentoRepository treinamentoRepository, ISessao sessao, MasterContext context, AulaRepository aulaRepository)
    {
        _usuariosRepository = usuariosRepository;
        _treinamentoRepository = treinamentoRepository;
        _sessao = sessao;
        _moduloRepository = moduloRepository;
        _context = context;
        _aulaRepository = aulaRepository;
    }
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
}

