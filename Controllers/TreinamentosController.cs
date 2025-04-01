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
public class TreinamentosController : Controller
{
    
    private readonly UsuariosRepository _usuariosRepository;
    private readonly TreinamentoRepository _treinamentoRepository;
    public TreinamentosController(UsuariosRepository usuariosRepository, TreinamentoRepository treinamentoRepository)
    {
        _treinamentoRepository = treinamentoRepository;
        _usuariosRepository = usuariosRepository;
    }

    [HttpPost]
    public IActionResult CriarTreinamento(Treinamento model)
    {
        var usuarioLogado = _usuariosRepository.ObterUsuarioLogado(HttpContext);
        if (usuarioLogado == null)
        {
            return RedirectToAction("Login", "Home");
        }

        if (!ModelState.IsValid)
        {
            return View(model); // Retorna a view se houver erro na validação
        }

        model.IdEmpresa = usuarioLogado.IdEmpresa;
        model.IdCriador = usuarioLogado.IdUsuarios;
        model.Entidades = usuarioLogado.IdEmpresaNavigation.Nome; // Nome da empresa
        
        _treinamentoRepository.Adicionar(model); // Salva no banco
        _treinamentoRepository.Salvar(); // Confirma a inserção

        return RedirectToAction("Index"); // Redireciona para a lista de treinamentos
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error!");
    }
}