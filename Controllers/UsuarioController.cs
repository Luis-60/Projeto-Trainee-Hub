using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;
using Projeto_Trainee_Hub.Helper;

namespace Projeto_Trainee_Hub.Controllers;

[Authorize(Roles = "1")]
public class UsuarioController : Controller
{
    
    private readonly ISessao _sessao;
    private readonly UsuariosRepository _usuariosRepository;
    public UsuarioController(UsuariosRepository usuariosRepository, ISessao sessao)
    {
        _usuariosRepository = usuariosRepository;
        _sessao = sessao;
    }

    public async Task<IActionResult> IndexAsync()
    {   
        var usuario = _sessao.BuscarSessaoUsuario();
        if (usuario == null)
        {
            return RedirectToAction("Login", "Home");
        }

        return View(usuario);
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public async Task<IActionResult> AulaAsync()
    {
        var usuario = _sessao.BuscarSessaoUsuario();
        if (usuario == null)
        {
            return RedirectToAction("Login", "Home");
        }
        return View(usuario);
    }
    
    public async Task<IActionResult> PerfilAsync()
    {
        var usuario = _sessao.BuscarSessaoUsuario();
        if (usuario == null)
        {
            return View();
        }
        
        var idUsuario = usuario.IdUsuarios;
        if (idUsuario != null)
        {
            var usuarios = await _usuariosRepository.GetByIdAsync(idUsuario);
        return View(usuarios);
        }
        
        return RedirectToAction("Index");
    }
    

    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public async Task<IActionResult> AtualizarPerfil(int idUsuarios, IFormFile imagemPerfil)
    {
        // Buscar o usuário no banco de dados usando o repositório
        var usuario = await _usuariosRepository.GetByIdAsync(idUsuarios);
        if (usuario == null)
        {
            return NotFound(); // Retorna 404 se o usuário não for encontrado
        }

        // Processar o upload da imagem
        if (imagemPerfil != null)
        {
            var directoryPath = Path.Combine("wwwroot", "images");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Renomear o arquivo para evitar conflitos e limitar o tamanho do nome
            var fileName = Path.GetFileNameWithoutExtension(imagemPerfil.FileName);
            var extension = Path.GetExtension(imagemPerfil.FileName);
            fileName = fileName.Length > 50 ? fileName.Substring(0, 50) : fileName; // Limitar o nome a 50 caracteres
            var newFileName = $"{fileName}{extension}";
            var filePath = Path.Combine(directoryPath, newFileName);

            // Salvar o arquivo no diretório
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imagemPerfil.CopyToAsync(stream);
            }

            // Gerar o caminho da imagem para salvar no banco de dados
            var imagePath = $"/images/{newFileName}";

            // Garantir que o caminho não exceda 100 caracteres
            if (imagePath.Length > 100)
            {
                imagePath = imagePath.Substring(0, 100);
            }

            // Atualizar o caminho da imagem no banco de dados
            usuario.Imagem = imagePath;
        }

        // Atualizar o usuário no banco de dados usando o repositório
        await _usuariosRepository.UpdateAsync(usuario);

        // Redirecionar de volta para a página de perfil
        return RedirectToAction("Perfil", "Usuario");
    }

} 