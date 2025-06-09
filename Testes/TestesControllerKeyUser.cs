using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Projeto_Trainee_Hub.Controllers;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Repository;
using Projeto_Trainee_Hub.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Projeto_Trainee_Hub.Helper;

namespace Testes
{
    public class TestesControllerKeyUser
    {
        private readonly Mock<UsuariosRepository> _mockUsuariosRepo;
        private readonly Mock<ModuloRepository> _mockModuloRepo;
        private readonly Mock<TreinamentoRepository> _mockTreinamentoRepo;
        private readonly Mock<AulaRepository> _mockAulaRepo;
        private readonly Mock<ISessao> _mockSessao;
        private readonly MasterContext _context;
        private readonly KeyUserController _controller;

        public TestesControllerKeyUser()
        {
            var options = new DbContextOptionsBuilder<MasterContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            _context = new MasterContext(options);

            _mockUsuariosRepo = new Mock<UsuariosRepository>(_context);
            _mockModuloRepo = new Mock<ModuloRepository>(_context);
            _mockTreinamentoRepo = new Mock<TreinamentoRepository>(_context);
            _mockAulaRepo = new Mock<AulaRepository>(_context);
            _mockSessao = new Mock<ISessao>();

            _controller = new KeyUserController(
                _mockUsuariosRepo.Object,
                _mockModuloRepo.Object,
                _mockTreinamentoRepo.Object,
                _mockSessao.Object,
                _context,
                _mockAulaRepo.Object
            );
        }

        [Fact]
        public async Task PerfilAsync_UsuarioNaoLogado_RedirecionaParaLogin()
        {
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns((Usuarios)null);
            var result = await _controller.PerfilAsync();
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

        [Fact]
        public async Task TreinamentosAsync_TreinamentoNaoEncontrado_RetornaNotFound()
        {
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(new Usuarios { IdUsuarios = 1 });

            var usuariosRepo = new UsuariosRepository(_context);
            var treinamentoRepo = new TreinamentoRepository(_context);

            var controller = new KeyUserController(
                usuariosRepo,
                _mockModuloRepo.Object,
                treinamentoRepo,
                _mockSessao.Object,
                _context,
                _mockAulaRepo.Object
            );

            var result = await controller.TreinamentosAsync(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Modulos_ModuloNaoEncontrado_RetornaNotFound()
        {
            var result = _controller.Modulos(999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void CriarUsuarios_Get_DeveRetornarViewComListas()
        {
            // Adicione tipos e setores diretamente no contexto
            _context.Tipos.Add(new Tipo { IdTipo = 1, Nome = "Tipo1" });
            _context.Setors.Add(new Setor { IdSetor = 1, Nome = "Setor1" });
            _context.SaveChanges();

            var usuariosRepo = new UsuariosRepository(_context);
            var controller = new KeyUserController(
                usuariosRepo,
                _mockModuloRepo.Object,
                _mockTreinamentoRepo.Object,
                _mockSessao.Object,
                _context,
                _mockAulaRepo.Object
            );

            var result = controller.CriarUsuarios();
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UsuarioSetorTipoViewModel>(viewResult.Model);
            Assert.NotEmpty(model.Tipos);
            Assert.NotEmpty(model.Setores);
        }

        [Fact]
        public async Task CriarUsuarios_Post_UsuarioNaoLogado_RedirecionaParaLogin()
        {
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns((Usuarios)null);
            var model = new UsuarioSetorTipoViewModel { Nome = "Teste", Email = "teste@teste.com", Matricula = "123", Senha = "123", IdSetor = 1, IdTipo = 1 };
            var result = await _controller.CriarUsuarios(model);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

        [Fact]
        public async Task CriarUsuarios_Post_Valido_RedirecionaParaGerirUsuarios()
        {
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(new Usuarios { IdEmpresa = 1 });
            var model = new UsuarioSetorTipoViewModel { Nome = "Teste", Email = "teste@teste.com", Matricula = "123", Senha = "123", IdSetor = 1, IdTipo = 1 };
            var result = await _controller.CriarUsuarios(model);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("GerirUsuarios", redirect.ActionName);
        }

        [Fact]
        public void GerirUsuarios_DeveRetornarViewComUsuarios()
        {
            var usuario = new Usuarios { IdUsuarios = 1, IdEmpresa = 1 };
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(usuario);
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
            var result = _controller.GerirUsuarios();
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IEnumerable<UsuarioSetorTipoViewModel>>(viewResult.Model);
        }

        [Fact]
        public async Task EditarUsuario_Post_AtualizaERedireciona()
        {
            var usuario = new Usuarios { IdUsuarios = 1, Nome = "Antigo", IdEmpresa = 1 };
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
            var viewModel = new UsuarioSetorTipoViewModel { UsuarioId = 1, Nome = "Novo", Email = "novo@teste.com", Matricula = "123", Senha = "123", IdSetor = 1, IdTipo = 1 };
            var result = await _controller.EditarUsuario(viewModel);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("GerirUsuarios", redirect.ActionName);
        }

        [Fact]
        public async Task ExcluirUsuario_Post_DeveRedirecionar()
        {
            var result = await _controller.ExcluirUsuario(1);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("GerirUsuarios", redirect.ActionName);
        }
    }
}