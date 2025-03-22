using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Projeto_Trainee_Hub.Models;

public partial class MasterContext : DbContext
{
    public MasterContext()
    {
    }

    public MasterContext(DbContextOptions<MasterContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aula> Aulas { get; set; }

    public virtual DbSet<Documento> Documentos { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<Modulo> Modulos { get; set; }

    public virtual DbSet<Setor> Setors { get; set; }

    public virtual DbSet<Tipo> Tipos { get; set; }

    public virtual DbSet<Treinamento> Treinamentos { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    public virtual DbSet<UsuariosTreinamento> UsuariosTreinamentos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=172.18.0.2;Database=master;User Id=sa;Password=\"YourStrong!Password123\";TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aula>(entity =>
        {
            entity.HasKey(e => e.IdAula).HasName("PK__Aula__D861CCCB1A9702EF");

            entity.ToTable("Aula");

            entity.Property(e => e.IdAula).HasColumnName("idAula");
            entity.Property(e => e.Descricao)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IdDocumento).HasColumnName("idDocumento");
            entity.Property(e => e.Nome)
                .HasMaxLength(120)
                .IsUnicode(false);

            entity.HasOne(d => d.IdDocumentoNavigation).WithMany(p => p.Aulas)
                .HasForeignKey(d => d.IdDocumento)
                .HasConstraintName("FK__Aula__idDocument__3118447E");
        });

        modelBuilder.Entity<Documento>(entity =>
        {
            entity.HasKey(e => e.IdDocumento).HasName("PK__Document__572A36FCD94D3237");

            entity.ToTable("Documento");

            entity.Property(e => e.IdDocumento).HasColumnName("idDocumento");
            entity.Property(e => e.Nome)
                .HasMaxLength(240)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.IdEmpresa).HasName("PK__Empresa__75D2CED42740CD93");

            entity.ToTable("Empresa");

            entity.Property(e => e.IdEmpresa).HasColumnName("idEmpresa");
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Descricao)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdUsuarios).HasColumnName("idUsuarios");
            entity.Property(e => e.Logo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefone)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUsuariosNavigation).WithMany(p => p.Empresas)
                .HasForeignKey(d => d.IdUsuarios)
                .HasConstraintName("FK__Empresa__idUsuar__3AA1AEB8");
        });

        modelBuilder.Entity<Modulo>(entity =>
        {
            entity.HasKey(e => e.IdModulos).HasName("PK__Modulos__E07DCF2BA7D30CF9");

            entity.Property(e => e.IdModulos).HasColumnName("idModulos");
            entity.Property(e => e.Descricao)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IdAula).HasColumnName("idAula");
            entity.Property(e => e.Nome)
                .HasMaxLength(120)
                .IsUnicode(false);

            entity.HasOne(d => d.IdAulaNavigation).WithMany(p => p.Modulos)
                .HasForeignKey(d => d.IdAula)
                .HasConstraintName("FK__Modulos__idAula__33F4B129");
        });

        modelBuilder.Entity<Setor>(entity =>
        {
            entity.HasKey(e => e.IdSetor).HasName("PK__Setor__A3780105AFEA2D87");

            entity.ToTable("Setor");

            entity.Property(e => e.IdSetor).HasColumnName("idSetor");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Tipo>(entity =>
        {
            entity.HasKey(e => e.IdTipo).HasName("PK__Tipo__BDD0DFE1B0E67F44");

            entity.ToTable("Tipo");

            entity.Property(e => e.IdTipo).HasColumnName("idTipo");
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Treinamento>(entity =>
        {
            entity.HasKey(e => e.IdTreinamentos).HasName("PK__Treiname__8F62BC539AB4842F");

            entity.Property(e => e.IdTreinamentos).HasColumnName("idTreinamentos");
            entity.Property(e => e.DataFim).HasColumnType("datetime");
            entity.Property(e => e.DataInicio).HasColumnType("datetime");
            entity.Property(e => e.Descricao)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Entidades)
                .HasMaxLength(240)
                .IsUnicode(false);
            entity.Property(e => e.IdEmpresa).HasColumnName("idEmpresa");
            entity.Property(e => e.IdModulos).HasColumnName("idModulos");
            entity.Property(e => e.Imagem)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Instrutor)
                .HasMaxLength(240)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Treinamentos)
                .HasForeignKey(d => d.IdEmpresa)
                .HasConstraintName("FK__Treinamen__idEmp__3E723F9C");

            entity.HasOne(d => d.IdModulosNavigation).WithMany(p => p.Treinamentos)
                .HasForeignKey(d => d.IdModulos)
                .HasConstraintName("FK__Treinamen__idMod__3D7E1B63");
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.IdUsuarios).HasName("PK__Usuarios__3940559AA9142AAD");

            entity.Property(e => e.IdUsuarios).HasColumnName("idUsuarios");
            entity.Property(e => e.Cargo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdSetor).HasColumnName("idSetor");
            entity.Property(e => e.IdTipo).HasColumnName("idTipo");
            entity.Property(e => e.Imagem)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Matricula)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Senha)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdSetorNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdSetor)
                .HasConstraintName("FK__Usuarios__idSeto__36D11DD4");

            entity.HasOne(d => d.IdTipoNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdTipo)
                .HasConstraintName("FK__Usuarios__idTipo__37C5420D");
        });

        modelBuilder.Entity<UsuariosTreinamento>(entity =>
        {
            entity.HasKey(e => new { e.IdUsuarios, e.IdTreinamentos }).HasName("PK__Usuarios__11B67E5FA0776C82");

            entity.ToTable("Usuarios_Treinamentos");

            entity.Property(e => e.IdUsuarios).HasColumnName("idUsuarios");
            entity.Property(e => e.IdTreinamentos).HasColumnName("idTreinamentos");
            entity.Property(e => e.DataInicio).HasColumnType("datetime");
            entity.Property(e => e.DataTermino).HasColumnType("datetime");
            entity.Property(e => e.StatusTreinamento)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdTreinamentosNavigation).WithMany(p => p.UsuariosTreinamentos)
                .HasForeignKey(d => d.IdTreinamentos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuarios___idTre__4242D080");

            entity.HasOne(d => d.IdUsuariosNavigation).WithMany(p => p.UsuariosTreinamentos)
                .HasForeignKey(d => d.IdUsuarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuarios___idUsu__414EAC47");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
