using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Projeto_Trainee_Hub.Helper;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;

namespace Projeto_Trainee_Hub.Controllers;


public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ISessao _sessao;
    private readonly UsuariosRepository _usuariosRepository;
    public HomeController(ILogger<HomeController> logger, UsuariosRepository usuariosRepository, ISessao sessao)
    {
        _logger = logger;
        _usuariosRepository = usuariosRepository;
        _sessao = sessao;
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
    
    public async Task<IActionResult> Perfil()
    {
        var matricula = HttpContext.Session.GetString("UsuarioMatricula");

        if (string.IsNullOrEmpty(matricula))
        {
            return RedirectToAction("Login", "Home");
        }

        var usuario = await _usuariosRepository.ObterPorMatriculaAsync(matricula);

        if (usuario == null)
        {
            return RedirectToAction("Login", "Home");
        }

        return View(usuario);
    }

    public IActionResult Login()
    {
        return View();
    }   
    
    [HttpPost]
    public async Task<IActionResult> Login(Usuarios usuario)
    {
        
        var usuarioExistente = _usuariosRepository.ValidarUsuario(usuario.Matricula, usuario.Senha);
        
        if (usuarioExistente == null)
        {
            ModelState.AddModelError(string.Empty, "Matrícula ou senha inválidos.");
            return View();
        }
        _sessao.CriarSessaoUsuario(usuarioExistente);
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuarioExistente.Matricula),
            new Claim(ClaimTypes.Name, usuarioExistente.Nome),
            new Claim(ClaimTypes.Role, usuarioExistente.IdTipo.ToString()),
            new Claim(ClaimTypes.Email, usuarioExistente.Email)

        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
        };
        var role = _sessao.BuscarSessaoUsuarioRole();
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
        
        );
        switch (_sessao.BuscarSessaoUsuarioRole())
        {
            case "1":
                return RedirectToAction("Index", "Usuario");
            case "2":
                return RedirectToAction("Perfil", "Admin");
            case "3":
                return RedirectToAction("Dashboard", "Gestor");
            case "4":
                return RedirectToAction("SSModulos", "Encarregado");
            default:
                return RedirectToAction("Login", "Home");
        };
        
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Home");
    }

    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
} 