using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace walletv2.Data.Entities.Objects;

public class RefreshToken : BaseEntityImplementation
{
    public Guid UserId { get; set; }

    [Required]
    public string? Token { get; set; }

    [Required]
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public bool IsUsed { get; set; }

    [ForeignKey("UserId")]
    public virtual User? User { get; set; } = default!;

}
