using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto_Trainee_Hub.Controllers;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;
using Projeto_Trainee_Hub.Helper;
using Projeto_Trainee_Hub.ViewModel;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Testes
{
    public class TestesControllerModulos
    {
        private readonly MasterContext _context;
        private readonly ModuloRepository _moduloRepo;
        private readonly TreinamentoRepository _treinamentoRepo;
        private readonly AulaRepository _aulaRepo;
        private readonly Mock<ISessao> _mockSessao;

        public TestesControllerModulos()
        {
            var options = new DbContextOptionsBuilder<MasterContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            _context = new MasterContext(options);
            _moduloRepo = new ModuloRepository(_context);
            _treinamentoRepo = new TreinamentoRepository(_context);
            _aulaRepo = new AulaRepository(_context);
            _mockSessao = new Mock<ISessao>();
        }

        [Fact]
        public async Task Deletar_IdZero_DeveRetornarNotFound()
        {
            var controller = new ModulosController(_mockSessao.Object, _moduloRepo, _treinamentoRepo, _context, _aulaRepo);
            var result = await controller.Deletar(0);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Deletar_ModuloComAulasComDocumentos_DeveRedirecionarComErro()
        {
            // Arrange
            var modulo = new Modulo { IdModulos = 1, Nome = "Teste", IdTreinamento = 1 };
            var aula = new Aula { IdAula = 1, IdModulo = 1, Nome = "Aula", Documentos = new List<Documento> { new Documento { IdDocumento = 1, Nome = "Doc" } } };
            _context.Modulos.Add(modulo);
            _context.Aulas.Add(aula);
            _context.SaveChanges();

            var controller = new ModulosController(_mockSessao.Object, _moduloRepo, _treinamentoRepo, _context, _aulaRepo);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.Request.Headers["Referer"] = "/anterior";

            var result = await controller.Deletar(1);
            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/anterior", redirect.Url);
        }

        [Fact]
        public async Task Editar_UsuarioNaoLogado_DeveRedirecionarParaLogin()
        {
            var controller = new ModulosController(_mockSessao.Object, _moduloRepo, _treinamentoRepo, _context, _aulaRepo);
            var viewModel = new TreinamentoModuloViewModel { modulos = new Modulo { IdModulos = 1 } };
            var result = await controller.Editar(viewModel);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

        [Fact]
        public async Task Editar_ModuloIdZero_DeveRetornarNotFound()
        {
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(new Usuarios { IdUsuarios = 1 });
            var controller = new ModulosController(_mockSessao.Object, _moduloRepo, _treinamentoRepo, _context, _aulaRepo);
            var viewModel = new TreinamentoModuloViewModel { modulos = new Modulo { IdModulos = 0 } };
            var result = await controller.Editar(viewModel);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Editar_ModuloNaoEncontrado_DeveRetornarNotFound()
        {
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(new Usuarios { IdUsuarios = 1 });
            var controller = new ModulosController(_mockSessao.Object, _moduloRepo, _treinamentoRepo, _context, _aulaRepo);
            var viewModel = new TreinamentoModuloViewModel { modulos = new Modulo { IdModulos = 999 } };
            var result = await controller.Editar(viewModel);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CriarModulo_IdTreinamentoNull_DeveRetornarNotFound()
        {
            var controller = new ModulosController(_mockSessao.Object, _moduloRepo, _treinamentoRepo, _context, _aulaRepo);
            var viewModel = new TreinamentoModuloViewModel { treinamentos = new Treinamento(), modulos = new Modulo() };
            var result = await controller.CriarModulo(viewModel);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CriarModulo_UsuarioNaoLogado_DeveRedirecionarParaLogin()
        {
            var controller = new ModulosController(_mockSessao.Object, _moduloRepo, _treinamentoRepo, _context, _aulaRepo);
            var viewModel = new TreinamentoModuloViewModel { treinamentos = new Treinamento { IdTreinamentos = 1 }, modulos = new Modulo() };
            var result = await controller.CriarModulo(viewModel);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }
    }
}
