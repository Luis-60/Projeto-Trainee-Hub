using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Projeto_Trainee_Hub.Models;
using Microsoft.AspNetCore.Authorization;
namespace Projeto_Trainee_Hub.Controllers;

[Authorize(Roles = "4")]
public class EncarregadoController : Controller
{

    public EncarregadoController()
    {
    }
    public IActionResult Perfil()
    {
        return View();
    }
    
    public IActionResult SSModulos()
    {
        return View();
    }

    public IActionResult SSAulas()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
