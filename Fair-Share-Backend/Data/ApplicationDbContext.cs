using System;
using System.Collections.Generic;
using Fair_Share_Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share_Backend.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountTask> AccountTasks { get; set; }

    public virtual DbSet<TeamTask> Tasks { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseNpgsql("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("account_pkey");
        });

        modelBuilder.Entity<AccountTask>(entity =>
        {
            entity.HasKey(e => new { e.AccountId, e.TaskId }).HasName("account_task_pkey");

            entity
                .HasOne(d => d.Account)
                .WithMany(p => p.AccountTasks)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("account_task_account_id_fkey");

            entity
                .HasOne(d => d.Task)
                .WithMany(p => p.AccountTasks)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("account_task_task_id_fkey");
        });

        modelBuilder.Entity<TeamTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("task_pkey");

            entity.Property(e => e.IsCompleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("team_pkey");
        });

        modelBuilder.Entity<TeamAccount>(entity =>
        {
            entity.HasKey(e => new { e.AccountId, e.TeamId }).HasName("team_account_pkey");

            entity.ToTable("team_account");

            entity.Property(e => e.AccountId).HasColumnName("account_id");

            entity.Property(e => e.TeamId).HasColumnName("team_id");

            entity
                .HasOne(e => e.Account)
                .WithMany(a => a.TeamAccounts)
                .HasForeignKey(e => e.AccountId)
                .HasConstraintName("team_account_account_id_fkey");

            entity
                .HasOne(e => e.Team)
                .WithMany(t => t.TeamAccounts)
                .HasForeignKey(e => e.TeamId)
                .HasConstraintName("team_account_team_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
