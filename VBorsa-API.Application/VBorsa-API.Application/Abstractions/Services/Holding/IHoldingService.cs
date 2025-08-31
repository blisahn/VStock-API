using VBorsa_API.Application.DTOs.Holding;
using VBorsa_API.Application.DTOs.Transaction;

namespace VBorsa_API.Application.Abstractions.Services.Holding;

public interface IHoldingService
{
    
    Task<Core.Entities.Holding> UpsertHoldingAsync(string userId, CreateHoldingDto holdingDto);
    Task UpdateCashHoldingAsync(string userId, decimal amountChange);
    Task<decimal> GetAssetQuantityAsync(string dtoSymbol, string userId);
    Task<IReadOnlyList<UserHoldingDto>> GetUserHoldingsAsync();
    
}