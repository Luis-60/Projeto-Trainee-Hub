using System;
using System.Collections.Generic;

namespace Projeto_Trainee_Hub.Models;

public partial class Empresa
{
    public int IdEmpresa { get; set; }

    public string? Nome { get; set; }

    public string? Logo { get; set; }

    public string? Descricao { get; set; }

    public string? Email { get; set; }

    public string? Telefone { get; set; }

    public string? Codigo { get; set; }

    public int? IdUsuarios { get; set; }

    public virtual Usuarios? IdUsuariosNavigation { get; set; }

    public virtual ICollection<Treinamento> Treinamentos { get; set; } = new List<Treinamento>();
}
