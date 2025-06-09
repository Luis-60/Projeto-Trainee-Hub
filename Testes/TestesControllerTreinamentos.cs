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
using System.IO;

namespace Testes
{
    public class TestesControllerTreinamentos
    {
        private readonly MasterContext _context;
        private readonly TreinamentoRepository _treinamentoRepo;
        private readonly ModuloRepository _moduloRepo;
        private readonly Mock<ISessao> _mockSessao;

        public TestesControllerTreinamentos()
        {
            var options = new DbContextOptionsBuilder<MasterContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            _context = new MasterContext(options);
            _treinamentoRepo = new TreinamentoRepository(_context);
            _moduloRepo = new ModuloRepository(_context);
            _mockSessao = new Mock<ISessao>();
        }

        [Fact]
        public async Task Deletar_IdZero_DeveRetornarNotFound()
        {
            var controller = new TreinamentosController(_treinamentoRepo, _mockSessao.Object, _context, _moduloRepo);
            var result = await controller.Deletar(0);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Deletar_TreinamentoNaoEncontrado_DeveRetornarNotFound()
        {
            var controller = new TreinamentosController(_treinamentoRepo, _mockSessao.Object, _context, _moduloRepo);
            var result = await controller.Deletar(123);
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task Editar_UsuarioNaoLogado_DeveRedirecionarParaLogin()
        {
            var controller = new TreinamentosController(_treinamentoRepo, _mockSessao.Object, _context, _moduloRepo);
            var viewModel = new TreinamentoUsuariosViewModel { treinamentos = new Treinamento { IdTreinamentos = 1 } };
            var result = await controller.Editar(viewModel);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

        [Fact]
        public async Task Editar_TreinamentoIdZero_DeveRetornarNotFound()
        {
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(new Usuarios { IdUsuarios = 1 });
            var controller = new TreinamentosController(_treinamentoRepo, _mockSessao.Object, _context, _moduloRepo);
            var viewModel = new TreinamentoUsuariosViewModel { treinamentos = new Treinamento { IdTreinamentos = 0 } };
            var result = await controller.Editar(viewModel);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Editar_TreinamentoNaoEncontrado_DeveRetornarNotFound()
        {
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(new Usuarios { IdUsuarios = 1 });
            var controller = new TreinamentosController(_treinamentoRepo, _mockSessao.Object, _context, _moduloRepo);
            var viewModel = new TreinamentoUsuariosViewModel { treinamentos = new Treinamento { IdTreinamentos = 999 } };
            var result = await controller.Editar(viewModel);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CriarTreinamento_UsuarioNaoLogado_DeveRedirecionarParaLogin()
        {
            var controller = new TreinamentosController(_treinamentoRepo, _mockSessao.Object, _context, _moduloRepo);
            var viewModel = new TreinamentoUsuariosViewModel { treinamentos = new Treinamento() };
            var result = await controller.CriarTreinamento(viewModel);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

        [Fact]
        public async Task CriarTreinamento_SemArquivo_DeveRetornarBadRequest()
        {
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(new Usuarios { IdUsuarios = 1 });
            var controller = new TreinamentosController(_treinamentoRepo, _mockSessao.Object, _context, _moduloRepo);
            var viewModel = new TreinamentoUsuariosViewModel { treinamentos = new Treinamento() };
            var result = await controller.CriarTreinamento(viewModel);
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}