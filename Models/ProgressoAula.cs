using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projeto_Trainee_Hub.Models
{
    [Table("ProgressoAulas")]
    public class ProgressoAula
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdAula { get; set; }
        public DateTime DataConclusao { get; set; }

        [ForeignKey("IdAula")]
        public virtual Aula Aula { get; set; }
        [ForeignKey("IdUsuario")]
        public virtual Usuarios Usuario { get; set; }

    }
}