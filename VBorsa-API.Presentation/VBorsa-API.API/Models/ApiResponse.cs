namespace VBorsa_API.Presentation.Models;

public sealed class ApiResponse<T>
{
    public bool Succeeded { get; init; }
    public string Message { get; init; }
    public T Data { get; init; }
    public string[] Errors { get; init; } = [];
}