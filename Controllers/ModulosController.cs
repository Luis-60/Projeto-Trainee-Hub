using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;
using Projeto_Trainee_Hub.ViewModel;
using Projeto_Trainee_Hub.Helper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Projeto_Trainee_Hub.Controllers;

public class ModulosController : Controller
{
    private readonly ISessao _sessao;
    private readonly AulaRepository _aulaRepository;
    private readonly ModuloRepository _moduloRepository;
    private readonly TreinamentoRepository _treinamentoRepository;
    private readonly MasterContext _context;
    public ModulosController(ISessao sessao, ModuloRepository moduloRepository, TreinamentoRepository treinamentoRepository,
     MasterContext context, AulaRepository aulaRepository)
    {
        _sessao = sessao;
        _moduloRepository = moduloRepository;
        _context = context;
        _treinamentoRepository = treinamentoRepository;
        _aulaRepository = aulaRepository;
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

        var aula = await _aulaRepository.GetByIdModuloAsync(idModulo);
        if (aula == null)
        {
            return NotFound();
        }
        if (aula.Any(a => a.Documentos.Any())) // Se algum módulo tiver aulas
        {
            TempData["Erro"] = "Não é possível deletar o treinamento, pois existem módulos com aulas.";
            return Redirect(Request.Headers["Referer"].ToString());
        }
        var modulo = await _moduloRepository.GetByIdAsync(idModulo);
        _context.Modulos.Remove(modulo);
        await _context.SaveChangesAsync();
        return Redirect(Request.Headers["Referer"].ToString());

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

        var modulos = _context.Modulos
        .Include(m => m.IdTreinamentoNavigation) // navega��o relacionada
        .FirstOrDefault(m => m.IdModulos == moduloId);

        modulos.IdModulos = modulos.IdModulos;
        modulos.Nome = treinamentoModulo.modulos.Nome;
        modulos.Descricao = treinamentoModulo.modulos.Descricao;


        await _moduloRepository.UpdateAsync(modulos);
        //_moduloRepository.Save();
        
        var idTreinamento = modulo.IdTreinamento;
        return Redirect(Request.Headers["Referer"].ToString());

    }
    // Criar Modulo
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarModulo(TreinamentoModuloViewModel treinamentoModulo)
    {
        var idTreinamento = treinamentoModulo.treinamentos.IdTreinamentos;
        if (idTreinamento == 0) // corrigido para int zero
        {
            return NotFound();
        }

        var usuario = _sessao.BuscarSessaoUsuario();
        if (usuario == null)
        {
            return RedirectToAction("Login", "Home");
        }

        treinamentoModulo.modulos.IdTreinamento = idTreinamento;

        // Calcula o último número de sequência para esse treinamento
        var ultimoSequencia = await _context.Modulos
            .Where(m => m.IdTreinamento == idTreinamento)
            .MaxAsync(m => (int?)m.Sequencia) ?? 0;

        treinamentoModulo.modulos.Sequencia = ultimoSequencia + 1;

        await _moduloRepository.AddAsync(treinamentoModulo.modulos);
        _moduloRepository.Save();
        var role = _sessao.BuscarSessaoUsuarioRole();

        var id = treinamentoModulo.modulos.IdModulos;
        if (role == "2")
        {
            return RedirectToAction("Modulos", "KeyUser", new { id });
        }
        if (role == "3")
        {
            return RedirectToAction("Modulos", "Gestor", new { id });
        }
        if (role == "4")
        {
            return RedirectToAction("Modulos", "Encarregado", new { id });
        }

        // Se não for nenhum dos casos acima, redireciona para o login
        return RedirectToAction("Login", "Home");
    }
}