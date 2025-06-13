using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Projeto_Trainee_Hub.Models;

namespace Projeto_Trainee_Hub.ViewModel
{
    public class AulaTreinamentosViewModel
    {
        public Usuarios usuarios { get; set; }
        public Aula aulas { get; set; }
        public Modulo modulos { get; set; }
        public IEnumerable<Aula> listaAulas { get; set; }
        public IEnumerable<Modulo> listaModulos { get; set; }
        public Documento documentos { get; set; }
        public Treinamento treinamento { get; set; }
        public UsuariosTreinamento usuariosTreinamento { get; set; }
    }
}