using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;
using Projeto_Trainee_Hub.Helper;

namespace Projeto_Trainee_Hub.Controllers;

[Authorize(Roles = "1")]
public class UsuarioController : Controller
{
    
    private readonly ISessao _sessao;
    private readonly UsuariosRepository _usuariosRepository;
    public UsuarioController(UsuariosRepository usuariosRepository, ISessao sessao)
    {
        _usuariosRepository = usuariosRepository;
        _sessao = sessao;
    }

    public async Task<IActionResult> IndexAsync()
    {   
        var usuario = _sessao.BuscarSessaoUsuario();
        if (usuario == null)
        {
            return RedirectToAction("Login", "Home");
        }

        return View(usuario);
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public async Task<IActionResult> AulaAsync()
    {
        var usuario = _sessao.BuscarSessaoUsuario();
        if (usuario == null)
        {
            return RedirectToAction("Login", "Home");
        }
        return View(usuario);
    }
    
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