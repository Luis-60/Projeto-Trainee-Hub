using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Projeto_Trainee_Hub.Models;

namespace Projeto_Trainee_Hub.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public AdminController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult SSTreinamentos()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
