using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Projeto_Trainee_Hub.ViewModel
{
    public class LoginViewModel
    {
        public string Matricula { get; set; }
        public string Senha { get; set; }
        public int IdEmpresa { get; set; }
        public bool LembrarMe { get; set; }
    }
}