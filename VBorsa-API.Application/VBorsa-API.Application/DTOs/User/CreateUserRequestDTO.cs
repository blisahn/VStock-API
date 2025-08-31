namespace VBorsa_API.Application.DTOs.User;

public class CreateUserRequestDTO
{
    public string Name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}