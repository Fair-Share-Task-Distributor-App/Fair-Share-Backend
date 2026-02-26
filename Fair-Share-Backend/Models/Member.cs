using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share_Backend.Models;

[Table("member")]
public partial class Member
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;

    [Column("email")]
    public string Email { get; set; } = null!;

    [InverseProperty("Member")]
    public virtual ICollection<MemberTask> MemberTasks { get; set; } = new List<MemberTask>();

    [ForeignKey("MemberId")]
    [InverseProperty("Members")]
    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
