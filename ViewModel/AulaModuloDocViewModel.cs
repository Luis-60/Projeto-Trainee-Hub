using System;
using System.Collections.Generic;
using Projeto_Trainee_Hub.Models;

namespace Projeto_Trainee_Hub.ViewModel
{
    public class AulaModuloDocViewModel
    {
        // Já existiam
        public Aula aulas { get; set; }
        public Modulo modulos { get; set; }
        public IEnumerable<Aula> listaAulas { get; set; }
        public Documento documentos { get; set; }

        // Adicionados para unificação com a tela de Treinamentos
        public Treinamento treinamentos { get; set; }
        public Usuarios usuarios { get; set; }
        public List<Modulo> listaModulos { get; set; } = new List<Modulo>();
    }
}
