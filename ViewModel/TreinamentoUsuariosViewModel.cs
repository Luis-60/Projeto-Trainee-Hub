using System;
using System.Collections.Generic;
using Projeto_Trainee_Hub.Models;

namespace Projeto_Trainee_Hub.ViewModel;

public class TreinamentoUsuariosViewModel
{
    public Treinamento treinamentos { get; set;}
    public Usuarios usuarios { get; set;}
    public IFormFile File { get; set; }
    public IEnumerable<Treinamento> listaTreinamentos { get; set; }
    public Dictionary<int, int> ProgressoPorTreinamento { get; set; }

}
