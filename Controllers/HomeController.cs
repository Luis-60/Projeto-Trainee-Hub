using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto_Trainee_Hub.Helper;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;
using Projeto_Trainee_Hub.ViewModel;
using System.Reflection;


namespace Projeto_Trainee_Hub.Controllers;

public class HomeController : Controller
{
    private readonly ISessao _sessao;
    private readonly UsuariosRepository _usuariosRepository;
    private readonly MasterContext _context;

    public HomeController(UsuariosRepository usuariosRepository, ISessao sessao, MasterContext context)
    {
        _usuariosRepository = usuariosRepository;
        _sessao = sessao;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Aula()
    {
        return View();
    }

    public async Task<IActionResult> Perfil()
    {
        System.Diagnostics.Debug.WriteLine("=== ENTROU NA FUNÇÃO PERFIL ===");
        var matricula = HttpContext.Session.GetString("UsuarioMatricula");

        if (string.IsNullOrEmpty(matricula))
        {
            return RedirectToAction("Login", "Home");
        }

        var usuario = await _usuariosRepository.GetByMatriculaAsync(matricula);

        if (usuario == null)
        {
            return RedirectToAction("Login", "Home");
        }

        var treinamentos = _context.Treinamentos
             .Where(t => t.IdEmpresa == usuario.IdEmpresa)
             .ToList();

        var progressoPorTreinamento = new Dictionary<int, int>();

        foreach (var treinamento in treinamentos)
        {
            var modulos = _context.Modulos
                .Where(m => m.IdTreinamento == treinamento.IdTreinamentos)
                .Select(m => m.IdModulos)
                .ToList();

            var aulasIds = _context.Aulas
                .Where(a => modulos.Contains(a.IdModulo))
                .Select(a => a.IdAula)
                .ToList();

            System.Diagnostics.Debug.WriteLine($"[DEBUG] Total de aulas no treinamento {treinamento.IdTreinamentos}: {aulasIds.Count}");
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Aulas totais vinculadas: {aulasIds.Count}");

            var totalAulas = aulasIds.Count;

            var concluidas = _context.ProgressoAulas
                .Count(p => aulasIds.Contains(p.IdAula) && p.IdUsuario == usuario.IdUsuarios);

            System.Diagnostics.Debug.WriteLine($"[DEBUG] Aulas concluídas pelo usuário {usuario.IdUsuarios}: {concluidas}");

            int progresso = (aulasIds.Count == 0) ? 0 : (int)Math.Round((double)concluidas * 100 / aulasIds.Count);
            progressoPorTreinamento[treinamento.IdTreinamentos] = progresso;
        }

        var viewModel = new TreinamentoUsuariosViewModel
        {
            usuarios = usuario,
            listaTreinamentos = treinamentos,
            ProgressoPorTreinamento = progressoPorTreinamento
        };

        return Content("ENTROU NA FUNÇÃO PERFIL");
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(Usuarios usuario)
    {
        var usuarioExistente = _usuariosRepository.ValidateUser(usuario.Matricula, usuario.Senha);

        if (usuarioExistente == null)
        {
            ModelState.AddModelError(string.Empty, "Matrícula ou senha inválidos.");
            return View();
        }

        _sessao.CriarSessaoUsuario(usuarioExistente);
        HttpContext.Session.SetString("UsuarioRole", usuarioExistente.IdTipo.ToString()); // 🟢 ALTERADO - salva o role na sessão

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuarioExistente.Matricula),
            new Claim(ClaimTypes.Name, usuarioExistente.Nome),
            new Claim(ClaimTypes.Role, usuarioExistente.IdTipo.ToString()), // 🟢 Já está correto
            new Claim(ClaimTypes.Email, usuarioExistente.Email)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
        );

        var role = _sessao.BuscarSessaoUsuarioRole(); // 🟢 Esse método já deve estar implementado na sua ISessao

        switch (role)
        {
            case "1":
                return RedirectToAction("Index", "Usuario");
            case "2":
                return RedirectToAction("Perfil", "KeyUser");
            case "3":
                return RedirectToAction("Dashboard", "Gestor");
            case "4":
                return RedirectToAction("Perfil", "Encarregado");
            case "5":
                return RedirectToAction("Administracao", "Admin"); // 🟢 ALTERADO - nova role para Admin
            default:
                return RedirectToAction("Login", "Home");
        }
    }

    public async Task<IActionResult> Logout()
    {
        _sessao.RemoverSessaoUsuario();

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync();

        return RedirectToAction("Login", "Home");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
