﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Projeto_Trainee_Hub.Models;

public partial class MasterContext : DbContext
{
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

    public virtual DbSet<ProgressoAula> ProgressoAulas { get; set; }

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
            entity.Property(e => e.Nome)
                .HasMaxLength(120)
                .IsUnicode(false);

            entity.HasOne(d => d.IdModuloNavigation).WithMany(p => p.Aulas)
                .HasForeignKey(d => d.IdModulo)
                .HasConstraintName("FK__Aula__idModulo__031C6FA4");
                
        });

        modelBuilder.Entity<Documento>(entity =>
        {
            entity.HasKey(e => e.IdDocumento).HasName("PK__Document__572A36FCD94D3237");

            entity.ToTable("Documento");

            entity.Property(e => e.IdDocumento).HasColumnName("idDocumento");
            entity.Property(e => e.Nome)
                .HasMaxLength(240)
                .IsUnicode(false);
            entity.HasOne(d => d.IdAulaNavigation).WithMany(p => p.Documentos)
                .HasForeignKey(d => d.IdAula)
                .HasConstraintName("FK__Documento__idAul__05F8DC4F");
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
            entity.Property(e => e.Logo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefone)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Modulo>(entity =>
        {
            entity.HasKey(e => e.IdModulos).HasName("PK__Modulos__E07DCF2BA7D30CF9");

            entity.Property(e => e.IdModulos).HasColumnName("idModulos");
            entity.Property(e => e.Descricao)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(120)
                .IsUnicode(false);

            entity.HasOne(d => d.IdTreinamentoNavigation).WithMany(p => p.Modulos)
                .HasForeignKey(d => d.IdTreinamento)
                .HasConstraintName("FK__Modulos__idTrein__1BE81D6E");
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
            entity.Property(e => e.Imagem)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Instrutor)
                .HasMaxLength(240)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.HasOne(d => d.IdCriadorNavigation).WithMany(p => p.Treinamentos)
                .HasForeignKey(d => d.IdCriador)
                .HasConstraintName("FK__Treinamen__idCri__7C6F7215");
                
            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Treinamentos)
                .HasForeignKey(d => d.IdEmpresa)
                .HasConstraintName("FK__Treinamen__idEmp__7D63964E");

        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.IdUsuarios).HasName("PK__Usuarios__3940559AA9142AAD");

            entity.Property(e => e.IdUsuarios).HasColumnName("idUsuarios");
            entity.Property(e => e.Nome).HasColumnName("Nome")
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdSetor).HasColumnName("idSetor");
            entity.Property(e => e.IdTipo).HasColumnName("idTipo");
            entity.Property(e => e.IdEmpresa).HasColumnName("idEmpresa");
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
                
            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdEmpresa)
                .HasConstraintName("FK__Usuarios__idEmpr__6E2152BE");
            
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
        modelBuilder.Entity<ProgressoAula>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProgressoAula");

            entity.ToTable("ProgressoAulas");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.IdAula).HasColumnName("idAula");
            entity.Property(e => e.DataConclusao).HasColumnType("datetime");

            entity.HasOne(d => d.Aula)
                .WithMany()
                .HasForeignKey(d => d.IdAula)
                .HasConstraintName("FK__ProgressoAula__idAula")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Usuario)
                .WithMany()
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__ProgressoAula__idUsuario")
                .OnDelete(DeleteBehavior.Cascade);
        });



        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}