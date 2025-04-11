using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Projeto_Trainee_Hub.Models;
namespace Projeto_Trainee_Hub.Controllers;
using Projeto_Trainee_Hub.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeto_Trainee_Hub.ViewModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Projeto_Trainee_Hub.Helper;
using Microsoft.EntityFrameworkCore;

public class TreinamentosController : Controller
{
    
    private readonly UsuariosRepository _usuariosRepository;
    private readonly TreinamentoRepository _treinamentoRepository;
    private readonly MasterContext _context;
    private readonly ISessao _sessao;
    private readonly string _baseUploadFolder = "wwwroot/images/upload/treinamentos";

    public TreinamentosController(UsuariosRepository usuariosRepository, TreinamentoRepository treinamentoRepository, ISessao sessao, MasterContext context)
    {
        if (!Directory.Exists(_baseUploadFolder))
        {
            Directory.CreateDirectory(_baseUploadFolder);
        }
        _treinamentoRepository = treinamentoRepository;
        _usuariosRepository = usuariosRepository;
        _sessao = sessao;
        _context = context;
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deletar(int IdTreinamentos)
    {
        if (IdTreinamentos == 0)
        {
            return NotFound();
        }

        var treinamento = await _treinamentoRepository.ObterPorIdAsync(IdTreinamentos);
        if (treinamento == null)
        {
            return NotFound();
        }

        _context.Treinamentos.Remove(treinamento);
        await _context.SaveChangesAsync();

        return RedirectToAction("Perfil", "Admin");
    }

    //função de editar treinamento
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(TreinamentoUsuariosViewModel treinamentoUsuarios)
    {
        var treinamentoId = treinamentoUsuarios.treinamentos.IdTreinamentos;
        var usuario = _sessao.BuscarSessaoUsuario();
        
        if (usuario == null)
        {
            return RedirectToAction("Login", "Home");
        }

        if (treinamentoId == 0)
        {
            return NotFound();
        }

        var treinamento = await _treinamentoRepository.ObterPorIdSemRastreamentoAsync(treinamentoId);
        if (treinamento == null)
        {
            return NotFound();
        }

        try
        {
            string fileName;

            // ✅ Novo arquivo foi enviado
            if (treinamentoUsuarios.File != null && treinamentoUsuarios.File.Length > 0)
            {
                fileName = Guid.NewGuid().ToString() + Path.GetExtension(treinamentoUsuarios.File.FileName);
                string filePath = Path.Combine(_baseUploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await treinamentoUsuarios.File.CopyToAsync(stream);
                }
            }
            else
            {
                // ✅ Nenhum novo arquivo enviado, mantém imagem existente
                fileName = treinamento.Imagem;
            }

            // ✅ Aplica o nome da imagem no objeto final
            treinamentoUsuarios.treinamentos.Imagem = fileName;

            // ✅ Salva as alterações
            await _treinamentoRepository.UpdateAsync(treinamentoUsuarios.treinamentos);
            _treinamentoRepository.Salvar();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TreinamentoExists(treinamentoId))
            {
                return NotFound();
            }

            throw; // Pode lançar novamente ou tratar de outra forma se quiser
        }
        var id = treinamentoUsuarios.treinamentos.IdTreinamentos;
        return RedirectToAction("SSModulos","Admin", new {id});
    
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarTreinamento(TreinamentoUsuariosViewModel treinamentoUsuarios)
    {
        var usuario = _sessao.BuscarSessaoUsuario();
        if (usuario == null)
        {
            return RedirectToAction("Login", "Home");
        }
        if (treinamentoUsuarios.File == null || treinamentoUsuarios.File.Length == 0)
        {
            return BadRequest("Nenhum Arquivo foi enviado");
        }
        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(treinamentoUsuarios.File.FileName);
        string filePath = Path.Combine(_baseUploadFolder, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await treinamentoUsuarios.File.CopyToAsync(stream);
        }
    
        treinamentoUsuarios.treinamentos.Imagem = fileName;
        
        _treinamentoRepository.Adicionar(treinamentoUsuarios.treinamentos); // Salva no banco
        _treinamentoRepository.Salvar(); // Confirma a inserção
        var id = treinamentoUsuarios.treinamentos.IdTreinamentos;
        return RedirectToAction("SSModulos","Admin", new {id});
    }

    public async Task<IActionResult> Detalhes(int id)
    {
        var treinamento = await _treinamentoRepository.ObterPorIdAsync(id);
        
        if (treinamento == null)
        {
            return NotFound();
        }

        // Obtém o usuário relacionado ao treinamento
        var usuario = treinamento.IdCriadorNavigation;

        // Cria o ViewModel
        var viewModel = new TreinamentoUsuariosViewModel
        {
            treinamentos = treinamento,
            usuarios = usuario ?? new Usuarios() // Caso o usuário não exista, cria uma instância vazia
        };

        return View(viewModel); // Passa o ViewModel para a View
    }

    private bool TreinamentoExists(int id)
        {
            return _context.Treinamentos.Any(e => e.IdTreinamentos == id);
        }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error!");
    }
}