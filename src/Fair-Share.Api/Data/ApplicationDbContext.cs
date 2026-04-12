using System;
using System.Collections.Generic;
using Fair_Share.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share.Api.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountTask> AccountTasks { get; set; }

    public virtual DbSet<Entities.Task> Tasks { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<AccountTaskPreference> AccountTaskPreferences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseNpgsql("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("account_pkey");
            entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("account_email_key");
            entity
                .HasOne(e => e.Team)
                .WithMany(t => t.Accounts)
                .HasForeignKey(e => e.TeamId)
                .IsRequired(false)
                .HasConstraintName("account_team_id_fkey");
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

        modelBuilder.Entity<Entities.Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("task_pkey");

            entity.Property(e => e.IsCompleted).HasDefaultValue(false);

            entity
                .HasOne(d => d.TeamOwned)
                .WithMany(p => p.Tasks)
                .HasForeignKey(d => d.TeamOwnedId)
                .HasConstraintName("task_team_owned_id_fkey");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("team_pkey");
        });

        modelBuilder.Entity<AccountTaskPreference>(entity =>
        {
            entity
                .HasKey(e => new { e.AccountId, e.TaskId })
                .HasName("account_task_preference_pkey");
            entity.ToTable("account_task_preference");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.Score).IsRequired().HasColumnName("score").HasMaxLength(50);
            entity
                .HasOne(e => e.Account)
                .WithMany(a => a.AccountTaskPreferences)
                .HasForeignKey(e => e.AccountId)
                .HasConstraintName("account_task_preference_account_id_fkey");
            entity
                .HasOne(e => e.Task)
                .WithMany(t => t.AccountTaskPreferences)
                .HasForeignKey(e => e.TaskId)
                .HasConstraintName("account_task_preference_task_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
