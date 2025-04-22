using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Projeto_Trainee_Hub.ViewModel
{
    public class UsuarioSetorTipoViewModel
    {
        public int UsuarioId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Matricula { get; set; }
        public string Senha { get; set; }
        public int? Idade { get; set; }

        // Ids selecionados no formulário
        public int? IdSetor { get; set; }
        public int? IdTipo { get; set; }
        public int IdEmpresa { get; set; }


        // Listas para preencher os dropdowns
        public List<SelectListItem> Setores { get; set; }
        public List<SelectListItem> Tipos { get; set; }
    }
}
