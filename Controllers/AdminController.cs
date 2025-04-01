using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;
using AspNetCoreGeneratedDocument;

[Authorize(Roles = "2")]
public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly UsuariosRepository _usuariosRepository;

    public AdminController(ILogger<AdminController> logger, UsuariosRepository usuariosRepository)
    {
        _logger = logger;
        _usuariosRepository = usuariosRepository;
    }
    public async Task<IActionResult> PerfilAsync(string matricula)
    {
        var usuarioExistente = await _usuariosRepository.ObterPorMatriculaAsync(matricula);
        return View(usuarioExistente);

    }
    public ViewComponentResult FormularioTreinamento()
    {
        return ViewComponent("Views_Admin_FormularioTreinamento");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
