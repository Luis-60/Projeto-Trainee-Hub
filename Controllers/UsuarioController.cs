using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;
using Projeto_Trainee_Hub.Helper;
using Projeto_Trainee_Hub.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Projeto_Trainee_Hub.Controllers;

[Authorize(Roles = "1")]
public class UsuarioController : Controller
{
    private readonly UsuariosRepository _usuariosRepository;
    private readonly ModuloRepository _moduloRepository;
    private readonly AulaRepository _aulaRepository;
    private readonly ISessao _sessao;
    private readonly TreinamentoRepository _treinamentoRepository;
    private readonly ModuloRepository _moduloRepository;
    private readonly AulaRepository _aulaRepository;
    private readonly MasterContext _context;

    public UsuarioController(UsuariosRepository usuariosRepository,ModuloRepository moduloRepository,
     ModuloRepository moduloRepository, TreinamentoRepository treinamentoRepository, AulaRepository aulaRepository, ISessao sessao, MasterContext context, AulaRepository aulaRepository)
    {
        _treinamentoRepository = treinamentoRepository;
        _sessao = sessao;
        _moduloRepository = moduloRepository;
        _context = context;
        _aulaRepository = aulaRepository;
    }

    public async Task<IActionResult> IndexAsync()
    {
        var usuario = _sessao.BuscarSessaoUsuario();
        if (usuario == null)
        {
            return RedirectToAction("Login", "Home");
        }

        int empresaId = (int)usuario.IdEmpresa;
        var progressoPorTreinamento = new Dictionary<int, int>();
        var treinamentosEmpresa = await _treinamentoRepository.GetTreinamentosEmpresa(empresaId);

        var treinamentoUsuarios = new TreinamentoUsuariosViewModel
        {
            treinamentos = new Treinamento(),
            usuarios = usuario,
            listaTreinamentos = treinamentosEmpresa,
            ProgressoPorTreinamento = progressoPorTreinamento
        };

        return View(treinamentoUsuarios);
    }

    public IActionResult Privacy()
    {
        return View();
    }
    
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> AulaAsync(AulaTreinamentosViewModel aulaTreinamentos)
        {
            // Obtém o ID do módulo (você precisa ajustar conforme a estrutura do seu ViewModel)
            var moduloId = aulaTreinamentos.modulos.IdModulos;
            var treinamentoId = aulaTreinamentos.treinamento.IdTreinamentos;
            // Busca o usuário logado
            var usuario = _sessao.BuscarSessaoUsuario();
            if (usuario == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Obtém os módulos do treinamento (supondo que você tenha um método que aceite o ID do treinamento)
            var listaModulos = await _moduloRepository.GetByIdTreinamentoAsync(treinamentoId);

            // Aqui você provavelmente precisa buscar também as aulas do módulo, então algo como:
            var listaAulas = await _aulaRepository.GetByIdModuloAsync(moduloId);

            // Prepara a ViewModel com os dados
            var viewModel = new AulaTreinamentosViewModel
            {
                usuarios = usuario,
                listaModulos = listaModulos,
                listaAulas = listaAulas
                
            };

            
            return View("Aula", viewModel);
        }

        //     {
        // public class AulaTreinamentosViewModel
        //         public Usuarios usuarios { get; set; }
        //         public Aula aulas { get; set; }
        //         public Modulo modulos { get; set; }
        //         public IEnumerable<Aula> listaAulas { get; set; }
        //         public IEnumerable<Modulo> listaModulos { get; set; }
        //         public Documento documentos { get; set; }
        //         public Treinamento treinamento { get; set; }
        //         public UsuariosTreinamento usuariosTreinamento { get; set; }
        //     }
        // }
    
    
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
            return View(usuarios);
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
        var idsModulos = modulosTreinamento.Select(m => m.IdModulos).ToList();
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
        // Busca o treinamento para associar ao ViewModel
        var treinamento = _context.Treinamentos.FirstOrDefault(t => t.IdTreinamentos == id);
        if (treinamento == null)
        {
            return NotFound();
        }

        // Busca os m�dulos relacionados ao treinamento
        var modulosDoTreinamento = _context.Modulos
                                .Where(m => m.IdTreinamento == id)
                                .OrderBy(m => m.Sequencia)
                                .ToList();

        // Busca as aulas dos m�dulos encontrados
        var idsModulos = modulosDoTreinamento.Select(m => m.IdModulos).ToList();
        var aulasDosModulos = _context.Aulas
                                .Where(a => idsModulos.Contains(a.IdModulo))
                                .Include(a => a.Documentos) // Inclui documentos se precisar mostrar
                                .ToList();

        var viewModel = new AulaModuloDocViewModel
        {
            treinamentos = treinamento,
            listaModulos = modulosDoTreinamento,
            listaAulas = aulasDosModulos,
            modulos = null,
            aulas = new Aula(),
            documentos = new Documento()
        };

        return View(viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public IActionResult Aulas(int idTreinamento)
    {
        var treinamento = _context.Treinamentos.FirstOrDefault(t => t.IdTreinamentos == idTreinamento);
        if (treinamento == null) return NotFound();

        // Pega todos os módulos do treinamento, ordenados pela sequência
        var todosModulos = _context.Modulos
            .Where(m => m.IdTreinamento == idTreinamento)
            .OrderBy(m => m.Sequencia)
            .ToList();

        if (!todosModulos.Any()) return NotFound("Nenhum módulo encontrado para esse treinamento.");

        var primeiroModulo = todosModulos.First();

        var idsModulos = todosModulos.Select(m => m.IdModulos).ToList();

        // Pega todas as aulas dos módulos
        var aulas = _context.Aulas
            .Where(a => idsModulos.Contains(a.IdModulo))
            .Include(a => a.Documentos)
            .OrderBy(a => a.IdModulo)
            .ToList();

        // Filtra para pegar as aulas do primeiro módulo
        var aulasDoPrimeiroModulo = aulas
            .Where(a => a.IdModulo == primeiroModulo.IdModulos)
            .ToList();

        var primeiraAula = aulasDoPrimeiroModulo.FirstOrDefault();

        var viewModel = new AulaModuloDocViewModel
        {
            treinamentos = treinamento,
            listaModulos = todosModulos,
            listaAulas = aulas,
            modulos = primeiroModulo,
            aulas = primeiraAula ?? new Aula()
        };

        return View(viewModel);
    }
}
