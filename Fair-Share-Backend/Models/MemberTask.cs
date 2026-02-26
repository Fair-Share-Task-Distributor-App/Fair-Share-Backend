using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share_Backend.Models;

[PrimaryKey("MemberId", "TaskId")]
[Table("member_task")]
public partial class MemberTask
{
    [Key]
    [Column("member_id")]
    public int MemberId { get; set; }

    [Key]
    [Column("task_id")]
    public int TaskId { get; set; }

    [Column("assigned_at", TypeName = "timestamp without time zone")]
    public DateTime AssignedAt { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("MemberTasks")]
    public virtual Member Member { get; set; } = null!;

    [ForeignKey("TaskId")]
    [InverseProperty("MemberTasks")]
    public virtual TeamTask Task { get; set; } = null!;
}
