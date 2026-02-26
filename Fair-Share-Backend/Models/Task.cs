using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share_Backend.Models;

[Table("task")]
public partial class TeamTask // RENAMED from Task
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    public string Title { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("due_at", TypeName = "timestamp without time zone")]
    public DateTime DueAt { get; set; }

    [Column("auto_assign_at", TypeName = "timestamp without time zone")]
    public DateTime AutoAssignAt { get; set; }

    [Column("is_completed")]
    public bool IsCompleted { get; set; }

    [InverseProperty("Task")]
    public virtual ICollection<MemberTask> MemberTasks { get; set; } = new List<MemberTask>();
}
