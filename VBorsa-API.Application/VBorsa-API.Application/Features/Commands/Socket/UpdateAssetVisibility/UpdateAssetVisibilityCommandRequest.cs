using MediatR;
using VBorsa_API.Application.DTOs.Helper;

namespace VBorsa_API.Application.Features.Commands.Socket.UpdateAssetVisibility;

public class UpdateAssetVisibilityCommandRequest : IRequest<Result>
{
    public bool IsActive { get; set; }
    public string Id { get; set; }
    public bool IsVisibleForNonLogin { get; set; }  
}