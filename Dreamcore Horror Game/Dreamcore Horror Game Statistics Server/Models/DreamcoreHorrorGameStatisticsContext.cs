using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DreamcoreHorrorGameStatisticsServer.Models;

public partial class DreamcoreHorrorGameStatisticsContext : DbContext
{
    public DreamcoreHorrorGameStatisticsContext()
    {
    }

    public DreamcoreHorrorGameStatisticsContext(DbContextOptions<DreamcoreHorrorGameStatisticsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Controller> Controllers { get; set; }

    public virtual DbSet<Method> Methods { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Sender> Senders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Controller>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("controllers_pkey");

            entity.ToTable("controllers");

            entity.HasIndex(e => e.Name, "controllers_unique_name").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Method>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("request_types_pkey");

            entity.ToTable("methods");

            entity.HasIndex(e => e.Name, "methods_unique_name").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("requests_pkey");

            entity.ToTable("requests");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ControllerId).HasColumnName("controller_id");
            entity.Property(e => e.MethodId).HasColumnName("method_id");
            entity.Property(e => e.ReceptionTimestamp).HasColumnName("reception_timestamp");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");

            entity.HasOne(d => d.Controller).WithMany(p => p.Requests)
                .HasForeignKey(d => d.ControllerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requests_fkey_controller_id");

            entity.HasOne(d => d.Method).WithMany(p => p.Requests)
                .HasForeignKey(d => d.MethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requests_fkey_method_id");

            entity.HasOne(d => d.Sender).WithMany(p => p.Requests)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requests_fkey_sender_id");
        });

        modelBuilder.Entity<Sender>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("senders_pkey");

            entity.ToTable("senders");

            entity.HasIndex(e => e.Name, "senders_unique_name").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
