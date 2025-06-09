using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto_Trainee_Hub.Controllers;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;
using Projeto_Trainee_Hub.Helper;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Testes
{
    public class TestesControllerHome
    {
        private readonly Mock<UsuariosRepository> _mockUsuariosRepo;
        private readonly Mock<ISessao> _mockSessao;
        private readonly MasterContext _context;
        private readonly HomeController _controller;

        public TestesControllerHome()
        {
            var options = new DbContextOptionsBuilder<MasterContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            _context = new MasterContext(options);
            _mockUsuariosRepo = new Mock<UsuariosRepository>(_context);
            _mockSessao = new Mock<ISessao>();

            _controller = new HomeController(_mockUsuariosRepo.Object, _mockSessao.Object, _context);
        }

        // Classe auxiliar para simular sessão em memória
        public class TestSession : ISession
        {
            private readonly Dictionary<string, byte[]> _sessionStorage = new();
            public IEnumerable<string> Keys => _sessionStorage.Keys;
            public string Id => "test";
            public bool IsAvailable => true;
            public void Clear() => _sessionStorage.Clear();
            public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public void Remove(string key) => _sessionStorage.Remove(key);
            public void Set(string key, byte[] value) => _sessionStorage[key] = value;
            public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);
        }

        [Fact]
        public void Index_DeveRetornarView()
        {
            var result = _controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_DeveRetornarView()
        {
            var result = _controller.Privacy();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Aula_DeveRetornarView()
        {
            var result = _controller.Aula();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Login_Get_DeveRetornarView()
        {
            var result = _controller.Login();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Perfil_SemUsuarioNaSessao_RedirecionaParaLogin()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession(); // Session fake
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            var result = await _controller.Perfil();
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
        }

        [Fact]
        public async Task Login_Post_UsuarioInvalido_RetornaViewComErro()
        {
            // Use o repositório real, não o mock, pois ValidateUser não é virtual
            var usuariosRepo = new UsuariosRepository(_context);
            var controller = new HomeController(usuariosRepo, _mockSessao.Object, _context);

            // Não adicione nenhum usuário ao contexto, assim ValidateUser sempre retorna null
            var usuario = new Usuarios { Matricula = "x", Senha = "y" };
            var result = await controller.Login(usuario);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public void Error_DeveRetornarView()
        {
            // Garante que o ControllerContext e HttpContext não são nulos
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            var result = _controller.Error();
            Assert.IsType<ViewResult>(result);
        }
    }


}
