using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto_Trainee_Hub.Controllers;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;
using Projeto_Trainee_Hub.Helper;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Testes
{
    public class TestesControllerUsuario
    {
        private readonly MasterContext _context;
        private readonly UsuariosRepository _usuariosRepo;
        private readonly Mock<ISessao> _mockSessao;

        public TestesControllerUsuario()
        {
            var options = new DbContextOptionsBuilder<MasterContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            _context = new MasterContext(options);
            _usuariosRepo = new UsuariosRepository(_context);
            _mockSessao = new Mock<ISessao>();
        }

        [Fact]
        public async Task IndexAsync_UsuarioNaoLogado_DeveRedirecionarParaLogin()
        {
            var controller = new UsuarioController(_usuariosRepo, _mockSessao.Object);
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns((Usuarios)null);

            var result = await controller.IndexAsync();
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

        [Fact]
        public async Task IndexAsync_UsuarioLogado_DeveRetornarViewComUsuario()
        {
            var usuario = new Usuarios { IdUsuarios = 1, Nome = "Teste" };
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(usuario);

            var controller = new UsuarioController(_usuariosRepo, _mockSessao.Object);
            var result = await controller.IndexAsync();
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(usuario, viewResult.Model);
        }

        [Fact]
        public void Privacy_DeveRetornarView()
        {
            var controller = new UsuarioController(_usuariosRepo, _mockSessao.Object);
            var result = controller.Privacy();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task AulaAsync_UsuarioNaoLogado_DeveRedirecionarParaLogin()
        {
            var controller = new UsuarioController(_usuariosRepo, _mockSessao.Object);
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns((Usuarios)null);

            var result = await controller.AulaAsync();
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

        [Fact]
        public async Task AulaAsync_UsuarioLogado_DeveRetornarViewComUsuario()
        {
            var usuario = new Usuarios { IdUsuarios = 2, Nome = "Aluno" };
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(usuario);

            var controller = new UsuarioController(_usuariosRepo, _mockSessao.Object);
            var result = await controller.AulaAsync();
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(usuario, viewResult.Model);
        }

        [Fact]
        public async Task PerfilAsync_UsuarioNaoLogado_DeveRetornarViewVazia()
        {
            var controller = new UsuarioController(_usuariosRepo, _mockSessao.Object);
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns((Usuarios)null);

            var result = await controller.PerfilAsync();
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        [Fact]
        public async Task PerfilAsync_UsuarioLogadoComId_DeveRetornarViewComUsuario()
        {
            var usuario = new Usuarios { IdUsuarios = 3, Nome = "Perfil" };
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(usuario);

            var controller = new UsuarioController(_usuariosRepo, _mockSessao.Object);
            var result = await controller.PerfilAsync();
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
            Assert.IsType<Usuarios>(viewResult.Model);
        }

        //[Fact]
        //public async Task PerfilAsync_UsuarioLogadoSemId_DeveRedirecionarParaIndex()
        //{
        //    var usuario = new Usuarios { IdUsuarios = 0 };
        //    _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(usuario);

        //    var controller = new UsuarioController(_usuariosRepo, _mockSessao.Object);
        //    var result = await controller.PerfilAsync();
        //    var redirect = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Index", redirect.ActionName);
        //}

        [Fact]
        public void Error_DeveRetornarView()
        {
            var controller = new UsuarioController(_usuariosRepo, _mockSessao.Object);
            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            var result = controller.Error();
            Assert.IsType<ViewResult>(result);
        }
    }
}