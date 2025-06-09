using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Projeto_Trainee_Hub.Controllers;
using Projeto_Trainee_Hub.Helper;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.ViewModel;
using Projeto_Trainee_Hub.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testes
{
    public class TestesControllerAdmin
    {

        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly Mock<ISessao> _mockSessao;
        private readonly MasterContext _context;
        private readonly AdminController _controller;
        private readonly Mock<EmpresaRepository> _mockEmpresaRepo;
        private readonly Mock<UsuariosRepository> _mockUsuariosRepo;
        private readonly Mock<ModuloRepository> _mockModuloRepo;
        private readonly Mock<TreinamentoRepository> _mockTreinamentoRepo;
        private readonly Mock<AulaRepository> _mockAulaRepo;


        public class FakeEmpresaRepository : EmpresaRepository
        {
            public FakeEmpresaRepository(MasterContext context) : base(context) { }

            // Marking the method as 'new' to hide the base class implementation
            public new Task<Empresa?> GetByIdAsync(int id) => Task.FromResult<Empresa?>(null);
        }

        private readonly EmpresaRepository _fakeEmpresaRepo;
        public TestesControllerAdmin()
        {

            var options = new DbContextOptionsBuilder<MasterContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Nome único para cada teste
            .Options;

            _context = new MasterContext(options);

            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockEnv.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

            _mockSessao = new Mock<ISessao>();
            _mockUsuariosRepo = new Mock<UsuariosRepository>(_context);
            _mockModuloRepo = new Mock<ModuloRepository>(_context);
            _mockTreinamentoRepo = new Mock<TreinamentoRepository>(_context);
            _mockAulaRepo = new Mock<AulaRepository>(_context);
            // Inicialize o mock para _mockEmpresaRepo
            _mockEmpresaRepo = new Mock<EmpresaRepository>(_context);
            _fakeEmpresaRepo = new FakeEmpresaRepository(_context);

            _controller = new AdminController(
             _mockUsuariosRepo.Object,
             _mockModuloRepo.Object,
             _mockTreinamentoRepo.Object,
             _mockSessao.Object,
             _context,
             _fakeEmpresaRepo
            );


            // Simula o ambiente HTTP
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Referer"] = "/anterior";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task Admin_SemUsuarioLogado_DeveRetornarView()
        {
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns((Usuarios)null);
            var result = await _controller.Admin();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
         public async Task Admin_ComUsuarioLogado_DeveRedirecionar()
        {
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(new Usuarios());
            var result = await _controller.Admin();
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Administracao", redirect.ActionName);
        }

        [Fact]
        public async Task AcoesEmpresas_EmpresaNaoEncontrada_DeveRetornarNotFound()
        {
            // Não é necessário setup, pois FakeEmpresaRepository já retorna null
            var controller = new AdminController(
                _mockUsuariosRepo.Object,
                _mockModuloRepo.Object,
                _mockTreinamentoRepo.Object,
                _mockSessao.Object,
                _context,
                _fakeEmpresaRepo 
            );
            var result = await controller.AcoesEmpresas(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CriarEmpresas_SemUsuarioLogado_DeveRedirecionarParaLogin()
        {
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns((Usuarios)null);
            var result = await _controller.CriarEmpresas(new EmpresaUsuarioSetorTipoViewModel());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

        [Fact]
        public async Task CriarEmpresas_ComUsuarioLogado_DeveRetornarRedirectAdministracao()
        {
            // Arrange
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(new Usuarios());
            var viewModel = new EmpresaUsuarioSetorTipoViewModel
            {
                NomeEmpresa = "Empresa Teste",
                EmailEmpresa = "empresa@teste.com",
                Telefone = "123456789",
                Codigo = "123"
            };

            // Act
            var result = await _controller.CriarEmpresas(viewModel);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Administracao", redirect.ActionName);
        }

        public class FakeEmpresaRepositoryEdit : EmpresaRepository
        {
            private readonly Empresa _empresa;
            public FakeEmpresaRepositoryEdit(MasterContext context, Empresa empresa) : base(context)
            {
                _empresa = empresa;
            }

            // Marking the method as 'new' instead of 'override' since the base method is not virtual
            public new Task<Empresa?> GetByIdAsync(int id) => Task.FromResult<Empresa?>(_empresa);
        }

        [Fact]
        public async Task EditarEmpresa_ComDadosValidos_DeveRetornarRedirectAdministracao()
        {
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(new Usuarios());
            var empresaExistente = new Empresa
            {
                IdEmpresa = 1,
                Nome = "Empresa Original",
                Email = "original@empresa.com",
                Telefone = "111111111",
                Codigo = "001"
            };

            // Adiciona a empresa ao contexto para garantir que ela está rastreada
            _context.Empresas.Add(empresaExistente);
            await _context.SaveChangesAsync();

            var fakeRepo = new FakeEmpresaRepositoryEdit(_context, empresaExistente);

            var viewModel = new EmpresaUsuarioSetorTipoViewModel
            {
                IdEmpresa = 1,
                NomeEmpresa = "Empresa Editada",
                EmailEmpresa = "editada@empresa.com",
                Telefone = "987654321",
                Codigo = "999"
            };

            var controller = new AdminController(
                _mockUsuariosRepo.Object,
                _mockModuloRepo.Object,
                _mockTreinamentoRepo.Object,
                _mockSessao.Object,
                _context,
                fakeRepo
            );

            var result = await controller.EditarEmpresa(viewModel);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Administracao", redirect.ActionName);
        }
    }
}
