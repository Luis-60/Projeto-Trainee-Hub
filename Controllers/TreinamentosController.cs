using Projeto_Trainee_Hub.Models;
namespace Projeto_Trainee_Hub.Controllers;
using Projeto_Trainee_Hub.Repository;
using Microsoft.AspNetCore.Mvc;
using Projeto_Trainee_Hub.ViewModel;
using Microsoft.EntityFrameworkCore;
using Projeto_Trainee_Hub.Helper;

public class TreinamentosController : Controller
{
    
    private readonly TreinamentoRepository _treinamentoRepository;
    private readonly MasterContext _context;
    private readonly ISessao _sessao;
    private readonly ModuloRepository _moduloRepository;
    private readonly string _baseUploadFolder = "wwwroot/images/upload/treinamentos";

    public TreinamentosController(TreinamentoRepository treinamentoRepository,
     ISessao sessao, MasterContext context, ModuloRepository moduloRepository)
    {
        if (!Directory.Exists(_baseUploadFolder))
        {
            Directory.CreateDirectory(_baseUploadFolder);
        }
        _treinamentoRepository = treinamentoRepository;
        _sessao = sessao;
        _moduloRepository = moduloRepository;
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

        var treinamento = await _treinamentoRepository.GetByIdAsync(IdTreinamentos);
        if (treinamento == null)
        {
            return NotFound();
        }
        // Verifica se existem módulos associados ao treinamento
        var modulos = await _moduloRepository.GetByIdTreinamentoAsync(IdTreinamentos);
        if (modulos.Any(m => m.Aulas.Any())) // Se algum módulo tiver aulas
        {
            TempData["Erro"] = "Não é possível deletar o treinamento, pois existem módulos com aulas.";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        _context.Treinamentos.Remove(treinamento);
        await _context.SaveChangesAsync();

        var role = _sessao.BuscarSessaoUsuarioRole();


        if (role == "2")
        {
            return RedirectToAction("Perfil", "KeyUser");
        }
        if (role == "3")
        {
            return RedirectToAction("Dashboard", "Gestor");
        }
        if (role == "4")
        {
            return RedirectToAction("Perfil", "Encarregado");
        }

        // Se não for nenhum dos casos acima, redireciona para o login
        return RedirectToAction("Login", "Home");
    }

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

        var treinamento = await _treinamentoRepository.GetByIdNoTrackingAsync(treinamentoId);
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
            _treinamentoRepository.Save();
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
        return RedirectToAction("Treinamentos","KeyUser", new {id});
    
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
        
        await _treinamentoRepository.AddAsync(treinamentoUsuarios.treinamentos); // Salva no banco
        _treinamentoRepository.Save(); // Confirma a inserção
        var id = treinamentoUsuarios.treinamentos.IdTreinamentos;
        var role = _sessao.BuscarSessaoUsuarioRole();

        if (role == "2")
        {
            return RedirectToAction("Treinamentos", "KeyUser", new { id });
        }
        if (role == "3")
        {
            return RedirectToAction("Treinamentos", "Gestor", new { id });
        }
        if (role == "4")
        {
            return RedirectToAction("Treinamentos", "Encarregado", new { id });
        }

        // Se não for nenhum dos casos acima, redireciona para o login
        return RedirectToAction("Login", "Home");

    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ComecarTreinamento(AulaTreinamentosViewModel aulaTreinamentos)
    {
        var usuario = _sessao.BuscarSessaoUsuario();
        if (usuario == null)
        {
            return RedirectToAction("Login", "Home");
        }

        return RedirectToAction("", ""); 
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