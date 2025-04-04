using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Projeto_Trainee_Hub.Models;

namespace Projeto_Trainee_Hub.Helper
{
    public interface ISessao
    {
        void CriarSessaoUsuario(Usuarios usuario);
        void RemoverSessaoUsuario();
        Usuarios BuscarSessaoUsuario();
        string BuscarSessaoUsuarioRole();
    }
}