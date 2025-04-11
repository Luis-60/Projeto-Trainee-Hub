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

public class TreinamentosController : Controller
{
    
    private readonly UsuariosRepository _usuariosRepository;
    private readonly TreinamentoRepository _treinamentoRepository;
    private readonly ISessao _sessao;
    private readonly string _baseUploadFolder = "wwwroot/images/upload/treinamentos";

    public TreinamentosController(UsuariosRepository usuariosRepository, TreinamentoRepository treinamentoRepository, ISessao sessao)
    {
        if (!Directory.Exists(_baseUploadFolder))
        {
            Directory.CreateDirectory(_baseUploadFolder);
        }
        _treinamentoRepository = treinamentoRepository;
        _usuariosRepository = usuariosRepository;
        _sessao = sessao;
    }

    public async Task<IActionResult< DeletarTreinamento(TreinamentoUsuariosViewModel treinamentosUsuarios)
    {
        return View()
    }

    [HttpPost]
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


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error!");
    }
}