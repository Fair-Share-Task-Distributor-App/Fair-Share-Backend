using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share_Backend.Entities;

[Table("account")]
public partial class Account
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("password")]
    public string Password { get; set; } = null!;

    [Column("google_id")]
    [AllowNull]
    public string? GoogleId { get; set; }

    [Column("team_id")]
    public int? TeamId { get; set; }

    [Column("password_hash")]
    public string PasswordHash { get; set; } = null!;

    [Column("points")]
    public int Points { get; set; }

    [InverseProperty("Account")]
    public virtual ICollection<AccountTask> AccountTasks { get; set; } = new List<AccountTask>();

    public virtual ICollection<AccountTaskPreference> AccountTaskPreferences { get; set; } =
        new List<AccountTaskPreference>();

    [ForeignKey("TeamId")]
    [InverseProperty("Accounts")]
    public virtual Team? Team { get; set; }
}
