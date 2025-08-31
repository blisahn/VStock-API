using Microsoft.AspNetCore.Identity;

namespace VBorsa_API.Core.Entities.Identity;

public class AppUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Holding> Holdings { get; set; } = new List<Holding>();
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenEndDate { get; set; }
}