using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share_Backend.Models;

[Table("team")]
public partial class Team
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [ForeignKey("TeamId")]
    [InverseProperty("Teams")]
    public virtual ICollection<Member> Members { get; set; } = new List<Member>();
}
