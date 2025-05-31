using System;
using System.Collections.Generic;

namespace Projeto_Trainee_Hub.Models;

public partial class Aula
{
    public int IdAula { get; set; }

    public string? Nome { get; set; }

    public string? Descricao { get; set; }
    
    public int IdModulo { get; set; }

    public virtual Modulo? IdModuloNavigation { get; set; }
    public virtual ICollection<Documento> Documentos { get; set; } = new List<Documento>();
    public string? VideoUrl { get; set; }

}
