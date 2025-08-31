namespace VBorsa_API.Application.DTOs.User;

public class UserDTO
{
    public Guid? Id { get; init; }
    public string UserName { get; init; }
    public string FullName { get; init; }
    public string Email { get; init; }
    public IList<string> Roles { get; init; }
}