using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share.Api.Entities;

[Table("team")]
public partial class Team
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [InverseProperty("Team")]
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    [InverseProperty("TeamOwned")]
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
