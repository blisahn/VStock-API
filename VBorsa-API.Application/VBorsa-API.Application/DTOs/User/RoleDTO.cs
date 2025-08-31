namespace VBorsa_API.Application.DTOs.User;

public class RoleDTO
{
    public string Id { get; set; }
    public IEnumerable<string> Roles { get; set; }
}