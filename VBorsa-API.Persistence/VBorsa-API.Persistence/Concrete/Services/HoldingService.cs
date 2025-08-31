using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VBorsa_API.Application.Abstractions.Services.Holding;
using VBorsa_API.Application.Abstractions.Services.Redis;
using VBorsa_API.Application.Abstractions.Services.Symbol;
using VBorsa_API.Application.DTOs.Holding;
using VBorsa_API.Application.DTOs.Transaction;
using VBorsa_API.Application.Exceptions;
using VBorsa_API.Application.Repositories.Holding;
using VBorsa_API.Application.Repositories.Transaction;
using VBorsa_API.Core.Entities;

namespace VBorsa_API.Persistence.Concrete.Services;

public class HoldingService : IHoldingService
{
    private readonly IHoldingReadRepository _holdingReadRepository;
    private readonly IHoldingWriteRepository _holdingWriteRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISymbolService _symbolService;
    private readonly IRedisService _redisService;
    private readonly ILogger<HoldingService> _logger;

    public HoldingService(IHoldingReadRepository holdingReadRepository, IHoldingWriteRepository holdingWriteRepository,
        ISymbolService symbolService, IHttpContextAccessor httpContextAccessor, IRedisService redisService,
        ILogger<HoldingService> logger)
    {
        _holdingReadRepository = holdingReadRepository;
        _holdingWriteRepository = holdingWriteRepository;
        _symbolService = symbolService;
        _httpContextAccessor = httpContextAccessor;
        _redisService = redisService;
        _logger = logger;
    }


    public async Task<Holding> UpsertHoldingAsync(string userId, CreateHoldingDto holdingDto)
    {
        // var userGuid = Guid.Parse(userId);
        var s = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (s == null)
        {
            _logger.LogError(
                $"Kullanıcı kayıt olmadan Sembol ID: {holdingDto.SymbolId} olan sembol  üzerinde işlem yapmak istiyor:{_httpContextAccessor.HttpContext!.TraceIdentifier}");
            throw new UnauthorizedAccessException("Lutfen once giris yapiniz");
        }

        var userGuid = Guid.Parse(s);
        var symbolGuid = Guid.Parse(holdingDto.SymbolId);
        var existingHolding = await _holdingReadRepository.Table.FirstOrDefaultAsync(h =>
            h.UserId == userGuid && h.SymbolId == symbolGuid);

        Holding resultHolding;

        if (existingHolding is null)
        {
            resultHolding = new Holding()
            {
                Id = Guid.NewGuid(),
                UserId = userGuid,
                SymbolId = symbolGuid,
                Quantity = holdingDto.Quantity,
                NetWorth = holdingDto.NetWorth,
                AvgCost = holdingDto.AvgCost
            };
            var result = await _holdingWriteRepository.AddAsync(resultHolding);
            if (!result)
            {
                _logger.LogError("{ResultHoldingSymbol} sisteme kayıt edilirken bir hata meydana geldi", resultHolding.Symbol);
                throw new AssetCreationExceptıon("Yeni varlık kaydı oluşturulurken bir hata ile karşılaşıldı.");
            }
        }
        else
        {
            if (holdingDto.Quantity > 0)
            {
                decimal newTotalQuantity = existingHolding.Quantity + holdingDto.Quantity;
                decimal newTotalNetWorth = existingHolding.NetWorth + holdingDto.NetWorth;

                existingHolding.Quantity = newTotalQuantity;
                existingHolding.NetWorth = newTotalNetWorth;
                existingHolding.AvgCost = newTotalQuantity == 0 ? 0 : newTotalNetWorth / newTotalQuantity;
            }
            else
            {
                decimal costOfSoldAssets = Math.Abs(holdingDto.Quantity) * (existingHolding.AvgCost ?? 0);

                existingHolding.Quantity += holdingDto.Quantity;
                existingHolding.NetWorth -= costOfSoldAssets;

                if (existingHolding.Quantity == 0)
                {
                    existingHolding.AvgCost = 0;
                    existingHolding.NetWorth = 0;
                }
            }

            _holdingWriteRepository.UpdateAsync(existingHolding);
            resultHolding = existingHolding;
        }

        await _holdingWriteRepository.SaveAsync();
        return resultHolding;
    }

    public async Task UpdateCashHoldingAsync(string userId, decimal amountChange)
    {
        var cashSymbol =
            await _symbolService.CheckIfSymbolExists("DEPOSIT",
                "USDT");
        if (cashSymbol is null)
        {
            _logger.LogCritical("Sistemsde $(USDT) tanımlı değil, lütfen sistem yöneticisi ile iletişime geçiniz");
            throw new Exception(
                "Sistemsel bir sorun mevcut lütfen temsilcisiniz ile iletişime geçiniz");
        }

        var userGuid = Guid.Parse(userId);

        var cashHolding = await _holdingReadRepository.GetSingleAsync(h =>
            h.UserId == userGuid && h.SymbolId.ToString() == cashSymbol.SymbolId);

        if (cashHolding is not null)
        {
            cashHolding.NetWorth += amountChange;
            cashHolding.Quantity += amountChange;
            _holdingWriteRepository.UpdateAsync(cashHolding);
        }
        else if (amountChange > 0)
        {
            await _holdingWriteRepository.AddAsync(new Holding
            {
                Id = Guid.NewGuid(),
                UserId = userGuid,
                SymbolId = Guid.Parse(cashSymbol.SymbolId),
                Quantity = amountChange,
                NetWorth = amountChange,
                AvgCost = 1
            });
        }

        await _holdingWriteRepository.SaveAsync();
    }

    public async Task<decimal> GetAssetQuantityAsync(string dtoSymbol, string userId)
    {
        var holding = await _holdingReadRepository.GetWhere(h => h.UserId == Guid.Parse(userId))
            .Include(h => h.Symbol)
            .FirstOrDefaultAsync(h => h.Symbol.Code == dtoSymbol);

        return holding?.Quantity ?? 0m;
    }

    public async Task<IReadOnlyList<UserHoldingDto>> GetUserHoldingsAsync()
    {
        var s = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (s == null)
        {
            _logger.LogInformation($"Kullanici giris yapmadan varliklara erismeye calisiyor {_httpContextAccessor.HttpContext!.TraceIdentifier}");
            throw new UnauthorizedAccessException("Lutfen once giris yapiniz");
        }

        var userGuid = Guid.Parse(s);
        var holdings = await _holdingReadRepository.Table
            .Where(h => h.UserId == userGuid && h.Quantity != 0)
            .Include(h => h.Symbol)
            .Where(h => h.Symbol.AssetClass != "DEPOSIT")
            .AsNoTracking()
            .ToListAsync();

        if (holdings.Count == 0)
            return new List<UserHoldingDto>();
        var results = new List<UserHoldingDto>(holdings.Count);
        foreach (var holding in holdings)
        {
            var currentPrice = await _redisService.GetLatestPriceBySymbol(holding.Symbol.Code);
            var changePercent = holding.AvgCost > 0
                ? ((currentPrice - holding.AvgCost.Value) / holding.AvgCost.Value) * 100
                : 0;
            var profitLoss = (currentPrice - (holding.AvgCost ?? 0)) * holding.Quantity;
            results.Add(new UserHoldingDto()
            {
                Symbol = holding.Symbol.Code,
                Name = holding.Symbol.AssetClass,
                Quantity = holding.Quantity,
                CurrentPrice = currentPrice,
                AverageCost = holding.AvgCost ?? 0m,
                MarketPrice = holding.Quantity * currentPrice,
                ChangePercent = changePercent,
                ProfitLoss = profitLoss
            });
        }

        return results;
    }
}