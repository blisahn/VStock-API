using Microsoft.AspNetCore.Identity;

namespace VBorsa_API.Core.Entities.Identity;

public class AppRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
}