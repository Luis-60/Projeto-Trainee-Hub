using System;
using System.Collections.Generic;

namespace Projeto_Trainee_Hub.Models;

public partial class Tipo
{
    public int IdTipo { get; set; }

    public string? Nome { get; set; }

    public virtual ICollection<Usuarios> Usuarios { get; set; } = new List<Usuarios>();
}
