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

[Authorize(Roles = "2")]
public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly UsuariosRepository _usuariosRepository;

    private readonly TreinamentoRepository _treinamentoRepository;

    public AdminController(ILogger<AdminController> logger, UsuariosRepository usuariosRepository, TreinamentoRepository treinamentoRepository)
    {
        _logger = logger;
        _usuariosRepository = usuariosRepository;
        _treinamentoRepository = treinamentoRepository;
    }
    [HttpGet]
    public async Task<IActionResult> PerfilAsync(string matricula)
    {
        
        var usuarioExistente = await _usuariosRepository.ObterPorMatriculaAsync(matricula);
        if (usuarioExistente == null)
        {
            return View();
        }
        var idUsuario = await _usuariosRepository.ObterIdPorMatriculaAsync(matricula);
        if (idUsuario != null)
        {
            var usuarioTreinamentos = await _treinamentoRepository.ObterPorIdCriadorAsync(idUsuario);
            var treinamentoUsuarios = new TreinamentoUsuariosViewModel{treinamentos = new Treinamento(), usuarios = usuarioExistente, listaTreinamentos = usuarioTreinamentos};

        return View(treinamentoUsuarios);
        }
        
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> SSModulosAsync(string matricula)
    {
        var usuarioExistente = await _usuariosRepository.ObterPorMatriculaAsync(matricula);
        if (usuarioExistente == null)
        {
            return RedirectToAction("Index");
        }
        return View(usuarioExistente);
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
