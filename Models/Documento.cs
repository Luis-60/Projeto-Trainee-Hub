using System;
using System.Collections.Generic;

namespace Projeto_Trainee_Hub.Models;

public partial class Documento
{
    public int IdDocumento { get; set; }

    public string? Nome { get; set; }

    public virtual ICollection<Aula> Aulas { get; set; } = new List<Aula>();
}
