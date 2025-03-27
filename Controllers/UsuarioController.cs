using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;

namespace Projeto_Trainee_Hub.Controllers;

[Authorize(Roles = "1")]
public class UsuarioController : Controller
{
    private readonly ILogger<UsuarioController> _logger;
    private readonly UsuariosRepository _usuariosRepository;
    public UsuarioController(ILogger<UsuarioController> logger, UsuariosRepository usuariosRepository)
    {
        _logger = logger;
        _usuariosRepository = usuariosRepository;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Aula()
    {
        return View();
    }

    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
} 