using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;

namespace Projeto_Trainee_Hub.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UsuariosRepository _usuariosRepository;
    public HomeController(ILogger<HomeController> logger, UsuariosRepository usuariosRepository)
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
    public IActionResult Perfil()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }   
    
    [HttpPost]
    public async Task<IActionResult> Login(Usuarios usuario)
    
    {
        if (ModelState.IsValid)
        {
            var usuarioExistente = await _usuariosRepository.LoginAsync(usuario.Matricula!, usuario.Senha!);                 
            
            if (usuarioExistente != null)
            {
                HttpContext.Session.SetString("UsuarioMatricula", usuarioExistente.Matricula!);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Matrícula ou senha inválida");
            }
            
        }
        return View(usuario);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
} 