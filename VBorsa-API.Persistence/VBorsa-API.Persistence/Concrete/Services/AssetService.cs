using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VBorsa_API.Application.Abstractions.Services.Asset;
using VBorsa_API.Application.Abstractions.Services.Holding;
using VBorsa_API.Application.Abstractions.Services.Redis;
using VBorsa_API.Application.Abstractions.Services.Transaction;
using VBorsa_API.Application.DTOs.Asset;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.Exceptions;

namespace VBorsa_API.Persistence.Concrete.Services;

public class AssetService : IAssetService
{
    private readonly IRedisService _redisService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITransactionService _transactionService;
    private readonly IHoldingService _holdingService;
    private readonly ILogger<AssetService> _logger;

    public AssetService(IRedisService redisService, IHttpContextAccessor httpContextAccessor,
        ITransactionService transactionService, IHoldingService holdingService, ILogger<AssetService> logger)
    {
        _redisService = redisService;
        _httpContextAccessor = httpContextAccessor;
        _transactionService = transactionService;
        _holdingService = holdingService;
        _logger = logger;
    }

    public async Task<Result> BuyAsset(BuyAssetRequestDto dto)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            _logger.LogError(
                $"Kullanici giris yapmadan degerli varlik satin almaya calisiyor {_httpContextAccessor.HttpContext?.TraceIdentifier} ");
            throw new UnauthorizedAccessException("Lutfen giris yapiniz");
        }

        decimal balance = await _transactionService.GetUserBalanceAsync(userId);
        var desiredAssetCost = await _redisService.GetLatestPriceBySymbol(dto.Symbol);
        if (desiredAssetCost * dto.Amount > balance)
            throw new TransactionException("Bakiyeniz bu islem icin yetersizdir");

        var res = await _transactionService.CreateBuyTransaction(userId, dto, desiredAssetCost);
        return Result.Success(res.Message);
    }

    public async Task<Result> SelAsset(SellAssetRequestDto dto)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            _logger.LogError(
                $"Kullanici giris yapmadan degerli varlik satmaya {_httpContextAccessor.HttpContext?.TraceIdentifier} ");
            throw new UnauthorizedAccessException("Lutfen giris yapiniz");
        }

        decimal ownedAssetQuantity = await _holdingService.GetAssetQuantityAsync(dto.Symbol, userId);
        var currentAssetPrice = await _redisService.GetLatestPriceBySymbol(dto.Symbol);
        if (ownedAssetQuantity < dto.Amount)
            throw new TransactionException("Degerli varliktan elinizde yeteri kadar yoktur");
        var res = await _transactionService.CreateSellTransaction(userId, dto, currentAssetPrice);
        return Result.Success(res.Message);
    }
}