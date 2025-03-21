using System;
using System.Collections.Generic;

namespace Projeto_Trainee_Hub.Models;

public partial class Aula
{
    public int IdAula { get; set; }

    public string? Nome { get; set; }

    public string? Descricao { get; set; }

    public int? IdDocumento { get; set; }

    public virtual Documento? IdDocumentoNavigation { get; set; }

    public virtual ICollection<Modulo> Modulos { get; set; } = new List<Modulo>();
}
