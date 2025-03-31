using System;
using System.Collections.Generic;

namespace Projeto_Trainee_Hub.Models;

public partial class Modulo
{
    public int IdModulos { get; set; }

    public string? Nome { get; set; }

    public string? Descricao { get; set; }

    public int? Sequencia { get; set; }
    public int IdTreinamento { get; set; }
    public virtual Treinamento? IdTreinamentoNavigation { get; set; }
    public virtual ICollection<Aula> Aulas { get; set; } = new List<Aula>();

}
