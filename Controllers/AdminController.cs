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

[Authorize(Roles = "2")]
public class AdminController : Controller
{
    private readonly UsuariosRepository _usuariosRepository;
    private readonly ISessao _sessao;
    private readonly TreinamentoRepository _treinamentoRepository;
    private readonly ModuloRepository _moduloRepository;
    public AdminController(UsuariosRepository usuariosRepository, ModuloRepository moduloRepository, TreinamentoRepository treinamentoRepository, ISessao sessao)
    {
        _usuariosRepository = usuariosRepository;
        _treinamentoRepository = treinamentoRepository;
        _sessao = sessao;
        _moduloRepository = moduloRepository;
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
}
