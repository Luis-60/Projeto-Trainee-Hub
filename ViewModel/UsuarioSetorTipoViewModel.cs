using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations; // Necessário para validações

namespace Projeto_Trainee_Hub.ViewModel
{
    public class UsuarioSetorTipoViewModel
    {
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Digite um email válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo Matrícula é obrigatório.")]
        public string Matricula { get; set; }

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        public string Senha { get; set; }

        [Range(18, 120, ErrorMessage = "A idade deve estar entre 18 e 120.")]
        public int? Idade { get; set; }

        public string Setor { get; set; } // Opcional, depende do uso

        public string Tipo { get; set; } // Opcional, depende do uso

        [Required(ErrorMessage = "O setor é obrigatório.")]
        public int? IdSetor { get; set; }

        [Required(ErrorMessage = "O tipo é obrigatório.")]
        public int? IdTipo { get; set; }

        public int IdEmpresa { get; set; }

        public List<SelectListItem> Setores { get; set; }
        public List<SelectListItem> Tipos { get; set; }
    }
}