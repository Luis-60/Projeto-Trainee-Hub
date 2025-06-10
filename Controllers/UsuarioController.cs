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
    private readonly TreinamentoRepository _treinamentoRepository;
    public UsuarioController(UsuariosRepository usuariosRepository, TreinamentoRepository treinamentoRepository, ISessao sessao)
    {
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