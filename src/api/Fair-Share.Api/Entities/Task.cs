using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share.Api.Entities;

[Table("task")]
public partial class Task
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

    [Column("points")]
    public int Points { get; set; }

    [Column("team_owned_id")]
    public int TeamOwnedId { get; set; }

    [ForeignKey("TeamOwnedId")]
    public Team TeamOwned { get; set; } = null!;

    [InverseProperty("Task")]
    public virtual ICollection<AccountTask> AccountTasks { get; set; } = new List<AccountTask>();

    public virtual ICollection<AccountTaskPreference> AccountTaskPreferences { get; set; } =
        new List<AccountTaskPreference>();
}
