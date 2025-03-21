using System;
using System.Collections.Generic;

namespace Projeto_Trainee_Hub.Models;

public partial class Modulo
{
    public int IdModulos { get; set; }

    public string? Nome { get; set; }

    public string? Descricao { get; set; }

    public int? Sequencia { get; set; }

    public int? IdAula { get; set; }

    public virtual Aula? IdAulaNavigation { get; set; }

    public virtual ICollection<Treinamento> Treinamentos { get; set; } = new List<Treinamento>();
}
