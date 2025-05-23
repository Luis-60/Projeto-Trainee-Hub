using System;
using System.Collections.Generic;

namespace Projeto_Trainee_Hub.Models;

public partial class Usuarios
{
    public int IdUsuarios { get; set; }
    public string? Nome { get; set; }

    public string? Email { get; set; }

    public string? Matricula { get; set; }

    public string? Senha { get; set; }
    
    public int? Idade { get; set; }

    public string? Imagem { get; set; }

    public int? QtdTreinamentos { get; set; }

    public int? IdSetor { get; set; }

    public int? IdTipo { get; set; }
    
    public int? IdEmpresa { get; set; }

    public virtual Setor? IdSetorNavigation { get; set; }

    public virtual Tipo? IdTipoNavigation { get; set; }

    public virtual Empresa? IdEmpresaNavigation { get; set; }
    public virtual ICollection<Treinamento> Treinamentos { get; set; } = new List<Treinamento>();
    public virtual ICollection<UsuariosTreinamento> UsuariosTreinamentos { get; set; } = new List<UsuariosTreinamento>();
}
