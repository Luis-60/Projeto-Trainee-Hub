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
    private readonly string _baseUploadFolder = "wwwroot/images/upload/treinamentos";

    public TreinamentosController(TreinamentoRepository treinamentoRepository,
     ISessao sessao, MasterContext context)
    {
        if (!Directory.Exists(_baseUploadFolder))
        {
            Directory.CreateDirectory(_baseUploadFolder);
        }
        _treinamentoRepository = treinamentoRepository;
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

        var treinamento = await _treinamentoRepository.GetByIdAsync(IdTreinamentos);
        if (treinamento == null)
        {
            return NotFound();
        }

        _context.Treinamentos.Remove(treinamento);
        await _context.SaveChangesAsync();

        return RedirectToAction("Perfil", "Admin");
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
        return RedirectToAction("Treinamentos","Admin", new {id});
    
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
        return RedirectToAction("Treinamentos","Admin", new {id});
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