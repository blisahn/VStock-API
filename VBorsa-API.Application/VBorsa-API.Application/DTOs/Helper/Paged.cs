namespace VBorsa_API.Application.DTOs.Helper;

public sealed record Paged<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize
);