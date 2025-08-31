using VBorsa_API.Application.DTOs.Asset;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.Transaction;

namespace VBorsa_API.Application.Abstractions.Services.Transaction;

public interface ITransactionService
{
    Task<IReadOnlyList<TransactionDto>> GetTransactionsByIdAsync(string? userId);
    Task<Result> CreateDepositAsync(CreateDepositDto createDepositDto);
    Task<decimal> GetUserBalanceAsync(string? userId);
    Task<Result> CreateBuyTransaction(string userId, BuyAssetRequestDto buyAssetRequestDto,decimal desiredAssetCost);
    Task<Result> CreateSellTransaction(string userId, SellAssetRequestDto sellAssetRequestDto, decimal currentAssetPrice);
}