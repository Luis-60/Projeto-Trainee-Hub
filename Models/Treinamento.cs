using System;
using System.Collections.Generic;

namespace Projeto_Trainee_Hub.Models;

public partial class Treinamento
{
    public int IdTreinamentos { get; set; }
    
    
    public string? Nome { get; set; }

    public string? Descricao { get; set; }

    public string? Imagem { get; set; }

    public DateTime? DataInicio { get; set; }

    public DateTime? DataFim { get; set; }

    public string? Instrutor { get; set; }

    public int? Duracao { get; set; }

    public string? Entidades { get; set; }

    public int? IdModulos { get; set; }

    public int? IdEmpresa { get; set; }
    public int? IdCriador { get; set; }

    public virtual Usuarios? IdCriadorNavigation { get; set;}
    
    public virtual Empresa? IdEmpresaNavigation { get; set; }

    public virtual Modulo? IdModulosNavigation { get; set; }

    public virtual ICollection<UsuariosTreinamento> UsuariosTreinamentos { get; set; } = new List<UsuariosTreinamento>();
}
