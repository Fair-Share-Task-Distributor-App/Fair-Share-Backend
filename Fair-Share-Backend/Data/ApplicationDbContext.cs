using System;
using System.Collections.Generic;
using Fair_Share_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share_Backend.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberTask> MemberTasks { get; set; }

    public virtual DbSet<TeamTask> Tasks { get; set; } // Changed to TeamTask

    public virtual DbSet<Team> Teams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseNpgsql("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("member_pkey");

            entity
                .HasMany(d => d.Teams)
                .WithMany(p => p.Members)
                .UsingEntity<Dictionary<string, object>>(
                    "TeamMember",
                    r =>
                        r.HasOne<Team>()
                            .WithMany()
                            .HasForeignKey("TeamId")
                            .HasConstraintName("team_member_team_id_fkey"),
                    l =>
                        l.HasOne<Member>()
                            .WithMany()
                            .HasForeignKey("MemberId")
                            .HasConstraintName("team_member_member_id_fkey"),
                    j =>
                    {
                        j.HasKey("MemberId", "TeamId").HasName("team_member_pkey");
                        j.ToTable("team_member");
                        j.IndexerProperty<int>("MemberId").HasColumnName("member_id");
                        j.IndexerProperty<int>("TeamId").HasColumnName("team_id");
                    }
                );
        });

        modelBuilder.Entity<MemberTask>(entity =>
        {
            entity.HasKey(e => new { e.MemberId, e.TaskId }).HasName("member_task_pkey");

            entity
                .HasOne(d => d.Member)
                .WithMany(p => p.MemberTasks)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("member_task_member_id_fkey");

            entity
                .HasOne(d => d.Task)
                .WithMany(p => p.MemberTasks)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("member_task_task_id_fkey");
        });

        modelBuilder.Entity<TeamTask>(entity => // Changed to TeamTask
        {
            entity.HasKey(e => e.Id).HasName("task_pkey");

            entity.Property(e => e.IsCompleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("team_pkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
