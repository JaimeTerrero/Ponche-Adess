using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Ponche_Adess.Domain.Data.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ResumenAsistenciaMensual> ResumenAsistenciaMensuals { get; set; }
    public virtual DbSet<AlertasAsistencium> AlertasAsistencia { get; set; }
    public virtual DbSet<Area> Areas { get; set; }
    public virtual DbSet<Empleado> Empleados { get; set; }
    public virtual DbSet<RegistrosAsistencium> RegistrosAsistencia { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Password=Des@rrollo2024;User ID=UserDesarrollo;TrustServerCertificate=True;Initial Catalog=ControlAsistenciaADESS;Data Source=172.22.95.29;MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ResumenAsistenciaMensual>(entity =>
        {
            entity.HasKey(e => e.ResumenId).HasName("PK__ResumenA__031582A5BF0F17E3");

            entity.ToTable("ResumenAsistenciaMensual");

            entity.HasIndex(e => new { e.EmpleadoId, e.Mes, e.Anio }, "UQ_EmpleadoPeriodo").IsUnique();

            entity.Property(e => e.ResumenId).HasColumnName("ResumenID");
            entity.Property(e => e.EmpleadoId).HasColumnName("EmpleadoID");
            entity.Property(e => e.HorasTrabajadas).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.PorcCumplimiento).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Seccion).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
