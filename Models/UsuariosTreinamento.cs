using System;
using System.Collections.Generic;

namespace Projeto_Trainee_Hub.Models;

public partial class UsuariosTreinamento
{
    public int IdTreinamentos { get; set; }

    public int IdUsuarios { get; set; }

    public double? Avaliacao { get; set; }

    public DateTime? DataInicio { get; set; }

    public DateTime? DataTermino { get; set; }

    public string? StatusTreinamento { get; set; }

    public double? Progresso { get; set; }

    public virtual Treinamento IdTreinamentosNavigation { get; set; } = null!;

    public virtual Usuarios IdUsuariosNavigation { get; set; } = null!;
}
