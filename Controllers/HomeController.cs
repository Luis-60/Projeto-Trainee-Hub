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
        // Valida as credenciais do usuário
        var usuarioExistente = _usuariosRepository.ValidarUsuario(usuario.Matricula, usuario.Senha);
        
        if (usuarioExistente != null)
        {
            // Armazena a matrícula do usuário na sessão
            HttpContext.Session.SetString("UsuarioMatricula", usuarioExistente.Matricula!);
            
            // Redireciona para a página inicial (ou outra área do sistema)
            return RedirectToAction("Index", "Home");
        }
        else
        {
            // Se o login falhar, adiciona um erro ao ModelState
            ModelState.AddModelError(string.Empty, "Matrícula ou senha inválida");
        }
    }
    
    // Se houver falha no login, exibe a tela de login novamente com a mensagem de erro
    return View(usuario);
}

    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
} 