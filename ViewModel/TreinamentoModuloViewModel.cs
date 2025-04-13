using System;
using System.Collections.Generic;
using Projeto_Trainee_Hub.Models;

namespace Projeto_Trainee_Hub.ViewModel;

public class TreinamentoModuloViewModel
{
    public Treinamento treinamentos { get; set;}
    public Modulo modulos { get; set;}
    public Usuarios usuarios{ get; set;}
    public IFormFile File { get; set; }
    public IEnumerable<Modulo> listaModulos { get; set; }

}
