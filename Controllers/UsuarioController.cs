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


namespace Projeto_Trainee_Hub.Controllers;

[Authorize(Roles = "1")]
public class UsuarioController : Controller
{
    
    private readonly ISessao _sessao;
    private readonly UsuariosRepository _usuariosRepository;
    private readonly ModuloRepository _moduloRepository;
    private readonly AulaRepository _aulaRepository;
    private readonly TreinamentoRepository _treinamentoRepository;
    public UsuarioController(UsuariosRepository usuariosRepository,ModuloRepository moduloRepository,
     TreinamentoRepository treinamentoRepository, AulaRepository aulaRepository, ISessao sessao)
    {
        _aulaRepository = aulaRepository;
        _moduloRepository = moduloRepository;
        _treinamentoRepository = treinamentoRepository;
        _usuariosRepository = usuariosRepository;
        _sessao = sessao;

    }

    public async Task<IActionResult> IndexAsync()
    {   
        var usuario = _sessao.BuscarSessaoUsuario();

        int EmpresaId = (int)usuario.IdEmpresa;

        var progressoPorTreinamento = new Dictionary<int, int>();
        var idEmpresa = usuario.IdEmpresaNavigation;
        var treinamentosEmpresa = await _treinamentoRepository.GetTreinamentosEmpresa(EmpresaId);
        if (usuario == null)        {
            return RedirectToAction("Login", "Home");
        }
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

    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
} 