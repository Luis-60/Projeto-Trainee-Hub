using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projeto_Trainee_Hub.Controllers;
using Projeto_Trainee_Hub.Models;
using Projeto_Trainee_Hub.ViewModel;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Testes
{
    public class TestesControllerEncarregado
    {
        private readonly MasterContext _context;

        public TestesControllerEncarregado()
        {
            var options = new DbContextOptionsBuilder<MasterContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            _context = new MasterContext(options);
        }

        [Fact]
        public void Perfil_DeveRetornarView()
        {
            var controller = new EncarregadoController(_context);
            var result = controller.Perfil();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Treinamentos_DeveRetornarView()
        {
            var controller = new EncarregadoController(_context);
            var result = controller.Treinamentos();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Modulos_IdInexistente_DeveRetornarNotFound()
        {
            var controller = new EncarregadoController(_context);
            var result = controller.Modulos(999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Modulos_IdExistente_DeveRetornarViewComViewModel()
        {
            var modulo = new Modulo { IdModulos = 1, Nome = "Modulo Teste", IdTreinamento = 1 };
            _context.Modulos.Add(modulo);
            _context.SaveChanges();

            var controller = new EncarregadoController(_context);
            var result = controller.Modulos(1);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<AulaModuloDocViewModel>(viewResult.Model);
        }

        [Fact]
        public void Error_DeveRetornarView()
        {
            var controller = new EncarregadoController(_context);
            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            var result = controller.Error();
            Assert.IsType<ViewResult>(result);
        }
    }
}