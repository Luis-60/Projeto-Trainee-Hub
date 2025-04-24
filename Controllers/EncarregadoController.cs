using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto_Trainee_Hub.Models;
using Microsoft.AspNetCore.Authorization;
using Projeto_Trainee_Hub.ViewModel;

namespace Projeto_Trainee_Hub.Controllers;

[Authorize(Roles = "4")]
public class EncarregadoController : Controller
{
    private readonly MasterContext _context;

    public EncarregadoController(MasterContext context)  
    {
        _context = context;
    }

    public IActionResult Perfil()
    {
        return View();
    }

    public IActionResult Treinamentos()
    {
        return View();
    }

    public IActionResult Modulos(int id)
    {
        var modulo = _context.Modulos.FirstOrDefault(m => m.IdModulos == id);

        if (modulo == null)
        {
            return NotFound();
        }

        var aulasDoModulo = _context.Aulas
                                    .Where(a => a.IdModulo == id)
                                    .ToList();

        var viewModel = new AulaModuloDocViewModel
        {
            modulos = modulo,
            listaAulas = aulasDoModulo,
            aulas = new Aula(), // Para o formul�rio do modal
            documentos = new Documento() // Para o formul�rio do modal
        };

        return View(viewModel);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

