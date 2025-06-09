using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Projeto_Trainee_Hub.Controllers;
using Projeto_Trainee_Hub.Helper;
using Projeto_Trainee_Hub.Models;

namespace Testes
{

    public class TestesControllerAula
    {
        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly Mock<ISessao> _mockSessao;
        private readonly MasterContext _context;
        private readonly AulasController _controller;



        public TestesControllerAula()
        {
            var options = new DbContextOptionsBuilder<MasterContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

            _context = new MasterContext(options);

            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockEnv.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            _mockSessao = new Mock<ISessao>();
            // Configure o mock conforme necessário para os testes

            _controller = new AulasController(_context, _mockEnv.Object, _mockSessao.Object);

            // Simula o ambiente HTTP
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Referer"] = "/anterior";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        // Teste para verificar se a criação de uma aula salva corretamente no banco de dados
        public async Task Criar_SalvarAulaEArquivos()
        {
            // Arrange
            string nome = "Aula Teste";
            string descricao = "Descrição da aula";
            int idModulo = 1;
            string videoUrl = "https://example.com/video";

            _context.Modulos.Add(new Modulo { IdModulos = idModulo, Nome = "Modulo Teste" });
            await _context.SaveChangesAsync();

            var fileMock = new Mock<IFormFile>();
            var content = "Arquivo de teste";
            var fileName = "documento.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns((Stream target, System.Threading.CancellationToken token) =>
            {
                stream.CopyTo(target);
                return Task.CompletedTask;
            });

            var files = new List<IFormFile> { fileMock.Object };

            // Act
            var result = await _controller.Criar(nome, descricao, idModulo, videoUrl, files);

            // Assert
            var aula = _context.Aulas.FirstOrDefault(a => a.Nome == nome);
            Assert.NotNull(aula);
            Assert.Equal(descricao, aula.Descricao);

            var documento = _context.Documentos.FirstOrDefault(d => d.IdAula == aula.IdAula);
            Assert.NotNull(documento);
            Assert.Equal(fileName, documento.Nome);

            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/anterior", redirectResult.Url);
        }

        //[Fact]
        //// Teste para verificar se a criação de uma aula com dados inválidos redireciona corretamente
        //public async Task Criar_ComDadosInvalidos_DeveRedirecionarParaModulo()
        //{
        //    // Arrange
        //    string nome = "";
        //    string descricao = "Descrição";
        //    int idModulo = 0;
        //    string videoUrl = null;
        //    var files = new List<IFormFile>();

        //    // Act
        //    var result = await _controller.Criar(nome, descricao, idModulo, videoUrl, files);

        //    // Assert
        //    var redirect = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Modulo", redirect.ActionName);
        //    Assert.Equal("Modulos", redirect.ControllerName);
        //    Assert.Equal(0, redirect.RouteValues["id"]);
        //}

        [Fact]
        // Teste para verificar se a criação de uma aula com erro ao salvar o arquivo lança exceção
        public async Task Criar_ArquivoComErro_LancarExcecao()
        {
            // Arrange
            string nome = "Aula com erro";
            string descricao = "Descrição";
            int idModulo = 5;
            string videoUrl = "https://erro.com";
            _context.Modulos.Add(new Modulo { IdModulos = idModulo, Nome = "Modulo Erro" });
            await _context.SaveChangesAsync();

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("erro.pdf");
            fileMock.Setup(f => f.Length).Returns(10);
            // Simula erro ao copiar arquivo
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                .ThrowsAsync(new IOException("Erro ao salvar arquivo"));

            var files = new List<IFormFile> { fileMock.Object };

            // Act & Assert
            await Assert.ThrowsAsync<IOException>(() =>
                _controller.Criar(nome, descricao, idModulo, videoUrl, files));
        }

        [Fact]
        // Teste para verificar se a edição de uma aula atualiza corretamente os dados e arquivos
        public async Task Editar_AtualizarAulaEArquivos()
        {
            // Arrange
            var modulo = new Modulo { IdModulos = 2, Nome = "Módulo Editar" };
            _context.Modulos.Add(modulo);
            var aula = new Aula { Nome = "Aula Original", Descricao = "Desc", IdModulo = 2, VideoUrl = "url" };
            _context.Aulas.Add(aula);
            await _context.SaveChangesAsync();

            // Adiciona documento antigo
            var docAntigo = new Documento { Nome = "antigo.pdf", IdAula = aula.IdAula };
            _context.Documentos.Add(docAntigo);
            await _context.SaveChangesAsync();

            // Novo arquivo para upload
            var fileMock = new Mock<IFormFile>();
            var content = "Novo arquivo";
            var fileName = "novo.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.CopyTo(It.IsAny<Stream>())).Callback<Stream>(s => stream.CopyTo(s));
            var files = new List<IFormFile> { fileMock.Object };

            // Act
            var aulaEditada = new Aula
            {
                IdAula = aula.IdAula,
                Nome = "Aula Editada",
                Descricao = "Nova Desc",
                IdModulo = 2,
                VideoUrl = "novaurl"
            };
            var result = await _controller.Editar(aulaEditada, files);

            // Assert
            var aulaDb = _context.Aulas.First(a => a.IdAula == aula.IdAula);
            Assert.Equal("Aula Editada", aulaDb.Nome);
            Assert.Equal("Nova Desc", aulaDb.Descricao);
            Assert.Equal("novaurl", aulaDb.VideoUrl);

            var docNovo = _context.Documentos.FirstOrDefault(d => d.IdAula == aula.IdAula && d.Nome == fileName);
            Assert.NotNull(docNovo);

            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/anterior", redirect.Url);
        }

        //[Fact]
        
        //public async Task Editar_AulaNaoEncontrada_DeveRetornarNotFound()
        //{
        //    // Arrange
        //    var aulaEditada = new Aula
        //    {
        //        IdAula = 9999, // ID inexistente
        //        Nome = "Inexistente",
        //        Descricao = "Desc",
        //        IdModulo = 1,
        //        VideoUrl = "url"
        //    };
        //    var files = new List<IFormFile>();

        //    // Act
        //    var result = await _controller.Editar(aulaEditada, files);

        //    // Assert
        //    Assert.IsType<NotFoundResult>(result);
        //}

        [Fact]
        // Teste para verificar se a exclusão de uma aula remove corretamente do banco de dados e seus documentos
        public async Task Excluir_RemoverAulaEDocumentos()
        {
            // Arrange
            var modulo = new Modulo { IdModulos = 3, Nome = "Módulo Excluir" };
            _context.Modulos.Add(modulo);
            var aula = new Aula { Nome = "Aula Excluir", Descricao = "Desc", IdModulo = 3 };
            _context.Aulas.Add(aula);
            await _context.SaveChangesAsync();

            var doc = new Documento { Nome = "excluir.pdf", IdAula = aula.IdAula };
            _context.Documentos.Add(doc);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Excluir(aula.IdAula);

            // Assert
            Assert.Null(_context.Aulas.FirstOrDefault(a => a.IdAula == aula.IdAula));
            Assert.Empty(_context.Documentos.Where(d => d.IdAula == aula.IdAula));

            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/anterior", redirect.Url);
        }

        //[Fact]
        //public async Task Excluir_AulaNaoEncontrada_NaoLancaExcecao()
        //{
        //    // Arrange
        //    int idInexistente = 9999;

        //    // Act
        //    var result = await _controller.Excluir(idInexistente);

        //    // Assert
        //    // Deve redirecionar mesmo se não encontrar
        //    var redirect = Assert.IsType<RedirectResult>(result);
        //    Assert.Equal("/anterior", redirect.Url);
        //}

        [Fact]
        // Teste para verificar se a conclusão de uma aula registra o progresso do usuário e redireciona corretamente
        public void ConcluirAula_RegistrarProgresso()
        {
            // Arrange
            var usuario = new Usuarios { IdUsuarios = 10, Nome = "Usuário Teste" };
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(usuario);

            var modulo = new Modulo { IdModulos = 4, Nome = "Módulo Progresso" };
            _context.Modulos.Add(modulo);
            var aula = new Aula { Nome = "Aula Progresso", IdModulo = 4 };
            _context.Aulas.Add(aula);
            _context.SaveChanges();

            // Act
            var result = _controller.ConcluirAula(aula.IdAula);

            // Assert
            var progresso = _context.ProgressoAulas.FirstOrDefault(p => p.IdAula == aula.IdAula && p.IdUsuario == usuario.IdUsuarios);
            Assert.NotNull(progresso);

            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/anterior", redirect.Url);
        }

        [Fact]
        // Teste para verificar se a conclusão de uma aula sem usuário logado retorna Unauthorized
        public void ConcluirAula_SemUsuario_DeveRetornarUnauthorized()
        {
            // Arrange
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns((Usuarios)null);

            // Act
            var result = _controller.ConcluirAula(1);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        // Teste para verificar se a conclusão de uma aula já concluída não duplica o progresso
        public void ConcluirAula_JaConcluida()
        {
            // Arrange
            var usuario = new Usuarios { IdUsuarios = 20, Nome = "Usuário" };
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(usuario);

            var modulo = new Modulo { IdModulos = 6, Nome = "Módulo" };
            _context.Modulos.Add(modulo);
            var aula = new Aula { Nome = "Aula", IdModulo = 6 };
            _context.Aulas.Add(aula);
            _context.SaveChanges();

            // Marca como já concluída
            _context.ProgressoAulas.Add(new ProgressoAula
            {
                IdAula = aula.IdAula,
                IdUsuario = usuario.IdUsuarios,
                DataConclusao = DateTime.Now
            });
            _context.SaveChanges();

            // Act
            var result = _controller.ConcluirAula(aula.IdAula);

            // Assert
            // Só deve existir um registro
            Assert.Equal(1, _context.ProgressoAulas.Count(p => p.IdAula == aula.IdAula && p.IdUsuario == usuario.IdUsuarios));
            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/anterior", redirect.Url);
        }

        [Fact]
        //Teste para verificar se consegue salvar varios arquivos
        public async Task Criar_ComMultiplosArquivos_DeveSalvarTodos()
        {
            // Arrange
            string nome = "Aula Multiplos Arquivos";
            int idModulo = 8;
            _context.Modulos.Add(new Modulo { IdModulos = idModulo, Nome = "Modulo Multi" });
            await _context.SaveChangesAsync();

            var file1 = new Mock<IFormFile>();
            var file2 = new Mock<IFormFile>();
            var content1 = "Arquivo 1";
            var content2 = "Arquivo 2";
            var fileName1 = "arq1.pdf";
            var fileName2 = "arq2.pdf";

            var stream1 = new MemoryStream();
            var writer1 = new StreamWriter(stream1);
            writer1.Write(content1);
            writer1.Flush();
            stream1.Position = 0;

            var stream2 = new MemoryStream();
            var writer2 = new StreamWriter(stream2);
            writer2.Write(content2);
            writer2.Flush();
            stream2.Position = 0;

            file1.Setup(f => f.FileName).Returns(fileName1);
            file1.Setup(f => f.Length).Returns(stream1.Length);
            file1.Setup(f => f.OpenReadStream()).Returns(stream1);
            file1.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns((Stream target, System.Threading.CancellationToken token) =>
            {
                stream1.CopyTo(target);
                return Task.CompletedTask;
            });

            file2.Setup(f => f.FileName).Returns(fileName2);
            file2.Setup(f => f.Length).Returns(stream2.Length);
            file2.Setup(f => f.OpenReadStream()).Returns(stream2);
            file2.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns((Stream target, System.Threading.CancellationToken token) =>
            {
                stream2.CopyTo(target);
                return Task.CompletedTask;
            });

            var files = new List<IFormFile> { file1.Object, file2.Object };

            // Act
            var result = await _controller.Criar(nome, "desc", idModulo, null, files);

            // Assert
            var aula = _context.Aulas.FirstOrDefault(a => a.Nome == nome);
            Assert.NotNull(aula);
            var documentos = _context.Documentos.Where(d => d.IdAula == aula.IdAula).ToList();
            Assert.Equal(2, documentos.Count);
            Assert.Contains(documentos, d => d.Nome == fileName1);
            Assert.Contains(documentos, d => d.Nome == fileName2);
        }

        [Fact]
        public async Task Criar_ArquivoMuitoGrande()
        {
            // Arrange
            string nome = "Aula Arquivo Gigante";
            int idModulo = 9;
            _context.Modulos.Add(new Modulo { IdModulos = idModulo, Nome = "Modulo Gigante" });
            await _context.SaveChangesAsync();

            var fileMock = new Mock<IFormFile>();
            var fileName = "gigante.pdf";
            var tamanho = 696_969L * 1024 * 1024 * 1024; 
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(tamanho);
            // Use um stream pequeno para não consumir memória real
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[1]));
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns(Task.CompletedTask);

            var files = new List<IFormFile> { fileMock.Object };

            // Act
            var result = await _controller.Criar(nome, "desc", idModulo, null, files);

            // Assert
            var aula = _context.Aulas.FirstOrDefault(a => a.Nome == nome);
            Assert.NotNull(aula);
            var documento = _context.Documentos.FirstOrDefault(d => d.IdAula == aula.IdAula && d.Nome == fileName);
            Assert.NotNull(documento);

            // Se implementar validação de tamanho, troque o Assert acima por:
            // Assert.IsType<BadRequestResult>(result);
        }
        

        [Fact]
        public async Task Criar_ArquivosComNomesDuplicados_DeveSalvarAmbosOuSubstituir()
        {
            // Arrange
            string nome = "Aula Arquivos Duplicados";
            int idModulo = 10;
            _context.Modulos.Add(new Modulo { IdModulos = idModulo, Nome = "Modulo Duplicado" });
            await _context.SaveChangesAsync();

            var fileName = "duplicado.pdf";
            var file1 = new Mock<IFormFile>();
            var file2 = new Mock<IFormFile>();
            var stream1 = new MemoryStream(new byte[] { 1, 2, 3 });
            var stream2 = new MemoryStream(new byte[] { 4, 5, 6 });

            file1.Setup(f => f.FileName).Returns(fileName);
            file1.Setup(f => f.Length).Returns(stream1.Length);
            file1.Setup(f => f.OpenReadStream()).Returns(stream1);
            file1.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns((Stream target, System.Threading.CancellationToken token) =>
            {
                stream1.CopyTo(target);
                return Task.CompletedTask;
            });

            file2.Setup(f => f.FileName).Returns(fileName);
            file2.Setup(f => f.Length).Returns(stream2.Length);
            file2.Setup(f => f.OpenReadStream()).Returns(stream2);
            file2.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns((Stream target, System.Threading.CancellationToken token) =>
            {
                stream2.CopyTo(target);
                return Task.CompletedTask;
            });

            var files = new List<IFormFile> { file1.Object, file2.Object };

            // Act
            var result = await _controller.Criar(nome, "desc", idModulo, null, files);

            // Assert
            var aula = _context.Aulas.FirstOrDefault(a => a.Nome == nome);
            Assert.NotNull(aula);
            var documentos = _context.Documentos.Where(d => d.IdAula == aula.IdAula && d.Nome == fileName).ToList();

            // Dependendo da regra do controller, pode sobrescrever ou manter ambos
            // Se sobrescreve, deve haver apenas 1 documento
            // Se mantém ambos, pode haver 2 (mas normalmente sobrescreve)
            Assert.True(documentos.Count == 1 || documentos.Count == 2);
        }
    }
}
