using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share_Backend.Entities;

[PrimaryKey("AccountId", "TaskId")]
[Table("account_task_preference")]
public class AccountTaskPreference
{
    [Key]
    [Column("account_id")]
    public int AccountId { get; set; }

    [Key]
    [Column("task_id")]
    public int TaskId { get; set; }

    [Column("score")]
    public int Score { get; set; }

    [ForeignKey("AccountId")]
    public Account Account { get; set; } = null!;

    [ForeignKey("TaskId")]
    public Task Task { get; set; } = null!;
}
