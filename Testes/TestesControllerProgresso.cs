using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto_Trainee_Hub.Controllers;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.Helper;
using Moq;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Testes
{
    public class TestesControllerProgresso
    {
        private readonly MasterContext _context;
        private readonly Mock<ISessao> _mockSessao;

        public TestesControllerProgresso()
        {
            var options = new DbContextOptionsBuilder<MasterContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new MasterContext(options);
            _mockSessao = new Mock<ISessao>();
        }

        [Fact]
        public void ProgressoTreinamento_UsuarioNaoLogado_DeveRedirecionarParaLogin()
        {
            var controller = new ProgressoController(_context, _mockSessao.Object);
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns((Usuarios)null);

            var result = controller.ProgressoTreinamento(1);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
            Assert.Equal("Home", redirect.ControllerName);
        }

        [Fact]
        public void ProgressoTreinamento_SemAulas_DeveRetornarZeroPorcento()
        {
            var usuario = new Usuarios { IdUsuarios = 1 };
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(usuario);

            var controller = new ProgressoController(_context, _mockSessao.Object);
            var result = controller.ProgressoTreinamento(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(0, controller.ViewBag.Porcentagem);
        }

        [Fact]
        public void ProgressoTreinamento_ComAulasEProgresso_DeveRetornarPorcentagemCorreta()
        {
            var usuario = new Usuarios { IdUsuarios = 1 };
            _mockSessao.Setup(s => s.BuscarSessaoUsuario()).Returns(usuario);

            // Adiciona 4 aulas no módulo 10
            _context.Aulas.Add(new Aula { IdAula = 1, IdModulo = 10 });
            _context.Aulas.Add(new Aula { IdAula = 2, IdModulo = 10 });
            _context.Aulas.Add(new Aula { IdAula = 3, IdModulo = 10 });
            _context.Aulas.Add(new Aula { IdAula = 4, IdModulo = 10 });
            // Marca 2 aulas como concluídas
            _context.ProgressoAulas.Add(new ProgressoAula { IdAula = 1, IdUsuario = 1, DataConclusao = DateTime.Now });
            _context.ProgressoAulas.Add(new ProgressoAula { IdAula = 2, IdUsuario = 1, DataConclusao = DateTime.Now });
            _context.SaveChanges();

            var controller = new ProgressoController(_context, _mockSessao.Object);
            var result = controller.ProgressoTreinamento(10);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(50, controller.ViewBag.Porcentagem);
        }

        [Fact]
        public void ConcluirAula_AulaJaConcluida_NaoAdicionaNovamente()
        {
            // Arrange
            _context.ProgressoAulas.Add(new ProgressoAula { IdAula = 1, IdUsuario = 1, DataConclusao = DateTime.Now });
            _context.SaveChanges();

            var controller = new ProgressoController(_context, _mockSessao.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.Request.Headers["Referer"] = "/anterior";

            var result = controller.ConcluirAula(1, 1);
            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/anterior", redirect.Url);

            // Só deve haver 1 registro
            Assert.Single(_context.ProgressoAulas.Where(p => p.IdAula == 1 && p.IdUsuario == 1));
        }

        [Fact]
        public void ConcluirAula_AulaNaoConcluida_AdicionaProgresso()
        {
            var controller = new ProgressoController(_context, _mockSessao.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.Request.Headers["Referer"] = "/anterior";

            var result = controller.ConcluirAula(2, 2);
            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/anterior", redirect.Url);

            // Deve haver 1 registro novo
            Assert.Single(_context.ProgressoAulas.Where(p => p.IdAula == 2 && p.IdUsuario == 2));
        }
    }
}
