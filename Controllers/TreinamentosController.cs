using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;
using System;
using System.IO;
using System.Threading.Tasks;

[Route("api/treinamentos")]
[ApiController]
public class TreinamentosController : Controller
{
    private readonly TreinamentoRepository _treinamentosRepository;
    public TreinamentosController(TreinamentoRepository treinamentoRepository)
    {
        _treinamentosRepository = treinamentoRepository;
        
    }

    [HttpPost]
    public async Task<IActionResult> CriarTreinamento([FromForm] Treinamento treinamento, IFormFile Imagem)
    {
        if (treinamento == null || Imagem == null)
        {
            return BadRequest("Todos os campos são obrigatórios.");
        }

        try
        {
            // Define o caminho da pasta onde a imagem será salva
            var pastaDestino = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/treinamentos");

            // Se a pasta não existir, cria ela
            if (!Directory.Exists(pastaDestino))
            {
                Directory.CreateDirectory(pastaDestino);
            }

            // Evita sobrescrever arquivos com o mesmo nome
            var nomeArquivo = $"{Guid.NewGuid()}_{Imagem.FileName}";
            var caminhoImagem = Path.Combine(pastaDestino, nomeArquivo);

            // Salva o arquivo no servidor
            using (var stream = new FileStream(caminhoImagem, FileMode.Create))
            {
                await Imagem.CopyToAsync(stream);
            }

            // Atualiza o caminho da imagem no banco de dados
            treinamento.Imagem = $"{nomeArquivo}";

            // Salva o treinamento no banco de dados
            _treinamentosRepository.UpdateAsync(treinamento);

            // Retorna a resposta com sucesso
            return CreatedAtAction(nameof(CriarTreinamento), new { id = treinamento.IdTreinamentos }, treinamento);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Erro ao criar treinamento: " + ex.Message);
        }
    }
}
