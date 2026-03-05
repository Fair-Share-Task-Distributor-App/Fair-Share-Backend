using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share_Backend.Entities;

[PrimaryKey("AccountId", "TaskId")]
[Table("account_task")]
public partial class AccountTask
{
    [Key]
    [Column("account_id")]
    public int AccountId { get; set; }

    [Key]
    [Column("task_id")]
    public int TaskId { get; set; }

    [Column("assigned_at", TypeName = "timestamp without time zone")]
    public DateTime AssignedAt { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("AccountTasks")]
    public virtual Account Account { get; set; } = null!;

    [ForeignKey("TaskId")]
    [InverseProperty("AccountTasks")]
    public virtual TeamTask Task { get; set; } = null!;
}
