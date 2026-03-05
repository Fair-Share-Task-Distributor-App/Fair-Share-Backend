using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fair_Share_Backend.Entities;
using Microsoft.EntityFrameworkCore;

[PrimaryKey("AccountId", "TeamId")]
[Table("team_account")]
public partial class TeamAccount
{
    [Key]
    [Column("account_id")]
    public int AccountId { get; set; }

    [Key]
    [Column("team_id")]
    public int TeamId { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("TeamAccounts")]
    public virtual Account Account { get; set; } = null!;

    [ForeignKey("TeamId")]
    [InverseProperty("TeamAccounts")]
    public virtual Team Team { get; set; } = null!;
}
