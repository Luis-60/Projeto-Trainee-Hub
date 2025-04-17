using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;
using Projeto_Trainee_Hub.ViewModel;
using Projeto_Trainee_Hub.Helper;

namespace Projeto_Trainee_Hub.Controllers;

public class ModulosController : Controller
{
    private readonly ISessao _sessao;
    private readonly ModuloRepository _moduloRepository;
    private readonly TreinamentoRepository _treinamentoRepository;
    private readonly MasterContext _context;
    public ModulosController(ISessao sessao, ModuloRepository moduloRepository, TreinamentoRepository treinamentoRepository,
     MasterContext context)
    {
        _sessao = sessao;
        _moduloRepository = moduloRepository;
        _context = context;
        _treinamentoRepository = treinamentoRepository;
    }

    // Deletar Modulo
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deletar(int idModulo)
    {
        if (idModulo == 0)
        {
            return NotFound();
        }

        var modulo = await _moduloRepository.GetByIdAsync(idModulo);
        if (modulo == null)
        {
            return NotFound();
        }
        _context.Modulos.Remove(modulo);
        await _context.SaveChangesAsync();

        var idTreinamento = modulo.IdTreinamento;

        return RedirectToAction("Treinamentos", "Admin", new {idTreinamento});
    }

    // Editar Modulo
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(TreinamentoModuloViewModel treinamentoModulo)
    {
        var moduloId = treinamentoModulo.modulos.IdModulos;
        var usuario = _sessao.BuscarSessaoUsuario();
        
        if (usuario == null)
        {
            return RedirectToAction("Login", "Home");
        }

        if (moduloId == 0)
        {
            return NotFound();
        }

        var modulo = await _moduloRepository.GetByIdAsync(moduloId);
        if (modulo == null)
        {
            return NotFound();
        }

        await _moduloRepository.UpdateAsync(modulo);
        _moduloRepository.Save();
        
        var idTreinamento = modulo.IdTreinamento;
        return RedirectToAction("Treinamentos", "Admin", new {idTreinamento});
    }
    // Criar Modulo
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarModulo(TreinamentoModuloViewModel treinamentoModulo)
    {
        var idTreinamento = treinamentoModulo.treinamentos.IdTreinamentos;
        if (idTreinamento == null)
        {
            return NotFound();
        }
        var usuario = _sessao.BuscarSessaoUsuario();
        if (usuario == null)
        {
            return RedirectToAction("Login", "Home");
        }
        treinamentoModulo.modulos.IdTreinamento = idTreinamento;
        await _moduloRepository.AddAsync(treinamentoModulo.modulos);
        _moduloRepository.Save();

        var id = treinamentoModulo.modulos.IdModulos;
        return RedirectToAction("Modulos", "Admin", new {id});
    }
}