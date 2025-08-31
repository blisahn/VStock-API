using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VBorsa_API.Application.Abstractions.Services.Holding;
using VBorsa_API.Application.Abstractions.Services.Symbol;
using VBorsa_API.Application.Abstractions.Services.Transaction;
using VBorsa_API.Application.DTOs.Asset;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.Holding;
using VBorsa_API.Application.DTOs.Transaction;
using VBorsa_API.Application.Exceptions;
using VBorsa_API.Application.Repositories.Transaction;
using VBorsa_API.Core.Entities;
using VBorsa_API.Core.Entities.Identity;

namespace VBorsa_API.Persistence.Concrete.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionReadRepository _transactionReadRepository;
    private readonly ITransactionWriteRepository _transactionWriteRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IHoldingService _holdingService;
    private readonly ISymbolService _symbolService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(ITransactionWriteRepository transactionWriteRepository,
        UserManager<AppUser> userManager, ITransactionReadRepository transactionReadRepository,
        IHoldingService holdingService, ISymbolService symbolService, IHttpContextAccessor httpContextAccessor,
        ILogger<TransactionService> logger)
    {
        _transactionWriteRepository = transactionWriteRepository;
        _userManager = userManager;
        _transactionReadRepository = transactionReadRepository;
        _holdingService = holdingService;
        _symbolService = symbolService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<IReadOnlyList<TransactionDto>> GetTransactionsByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogError($"Kullanıcı ID: {userId} sistemde kayıtlı değil");
            throw new UserNotFoundException("Kullanıcı işlemleri görüntülenemedi.");
        }

        var transactions = await _transactionReadRepository.Table.AsNoTracking()
            .Include(t => t.Symbol)
            .Where(t => t.UserId == Guid.Parse(userId) && !((t.Type.Contains("TRANSFERRED_TO_BUY:") ||
                                                             t.Type.Contains("TRANSFERRED_TO_SELL:"))))
            .ToListAsync();

        var results = new List<TransactionDto>(transactions.Count);
        foreach (var transaction in transactions)
        {
            results.Add(new TransactionDto
                {
                    Symbol = transaction.Symbol.Code,
                    Quantity = transaction.Quantity,
                    ExecutedAt = transaction.ExecutedAt,
                    Type = transaction.Type,
                    Price = transaction.Price,
                }
            );
        }

        return results;
    }

    public async Task<Result> CreateDepositAsync(CreateDepositDto createDepositDto)
    {
        var userIdStr = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdStr))
        {
            _logger.LogError(
                $"Kullanici giris yapmadan para yatirmaya calisiyor TraceID: {_httpContextAccessor.HttpContext.TraceIdentifier}");
            throw new UnauthorizedAccessException("Islemi yapmak icin once giris yapmalisiniz");
        }

        var userId = Guid.Parse(userIdStr);
        createDepositDto.UserId = userId;
        var symbol = await _symbolService.CheckIfSymbolExists(createDepositDto.TransactionType, createDepositDto.Code);

        Guid symbolId;
        if (symbol is null)
        {
            createDepositDto.SymbolId = Guid.NewGuid();
            await _symbolService.CreateDepositSymbolDto(createDepositDto);
            symbolId = createDepositDto.SymbolId;
        }
        else
        {
            symbolId = Guid.Parse(symbol.SymbolId);
        }

//TODO: DUZENLEME GEREKEBILIR
        var tx = new Transaction
        {
            Id = Guid.NewGuid(),
            SymbolId = symbolId,
            Quantity = createDepositDto.Price,
            ExecutedAt = DateTime.UtcNow,
            UserId = userId,
            Type = createDepositDto.TransactionType,
            Price = createDepositDto.Price
        };

        await _transactionWriteRepository.AddAsync(tx);
        await _transactionWriteRepository.SaveAsync();

        await _holdingService.UpdateCashHoldingAsync(userIdStr, createDepositDto.Price);
        await _transactionWriteRepository.SaveAsync();
        return Result.Success("Para yatirma islemi tamamlandi");
    }

    public async Task<decimal> GetUserBalanceAsync(string? userId)
    {
        var q = _transactionReadRepository.Table
            .Where(t => t.UserId == Guid.Parse(userId!));

        var deposits = await q.Where(t => t.Type.ToString() == "DEPOSIT")
            .SumAsync(t => (decimal?)t.Quantity) ?? 0m;

        var withdrawals = await q.Where(t => t.Type == "WITHDRAWAL")
            .SumAsync(t => (decimal?)t.Price) ?? 0m;

        var buyCost = await q.Where(t => t.Type == "BUY")
            .SumAsync(t => (decimal?)(t.Price * t.Quantity)) ?? 0m;

        var sellProceeds = await q.Where(t => t.Type == "SELL")
            .SumAsync(t => (decimal)t.Price * t.Quantity);

        var balance = deposits + sellProceeds
                      - buyCost - withdrawals;

        return balance;
    }

    public async Task<Result> CreateSellTransaction(string userId, SellAssetRequestDto sellAssetRequestDto,
        decimal currentAssetPrice)
    {
        try
        {
            var assetToSellSymbol =
                await _symbolService.CheckIfSymbolExists(sellAssetRequestDto.AssetClass, sellAssetRequestDto.Symbol);

            if (assetToSellSymbol is null)
                throw new TransactionException("Islem yapmak istediginiz degerli varlik mevcut degil");

            var transaction = new Transaction()
            {
                UserId = Guid.Parse(userId!),
                ExecutedAt = DateTime.UtcNow,
                SymbolId = Guid.Parse(assetToSellSymbol.SymbolId),
                Id = Guid.NewGuid(),
                Quantity = sellAssetRequestDto.Amount,
                Price = currentAssetPrice,
                Type = "SELL",
            };


            var res = await _transactionWriteRepository.AddAsync(transaction);
            var cashSymbol = await _symbolService.CheckIfSymbolExists("DEPOSIT", "USDT");
            if (cashSymbol is null)
                throw new TransactionCreationFailedException("Nakit sembolu ('USDT') tanimli degil.");
            var cashTransferTransaction = new Transaction()
            {
                UserId = Guid.Parse(userId),
                ExecutedAt = DateTime.UtcNow,
                SymbolId = Guid.Parse(cashSymbol.SymbolId),
                Id = Guid.NewGuid(),
                Quantity = currentAssetPrice * sellAssetRequestDto.Amount,
                Price = 1,
                Type = $"TRANSFERRED_TO_SELL:{assetToSellSymbol.Code}",
            };

            await _transactionWriteRepository.AddAsync(cashTransferTransaction);
            var holdingDto = new CreateHoldingDto()
            {
                SymbolId = assetToSellSymbol.SymbolId,
                Quantity = -sellAssetRequestDto.Amount,
                NetWorth = -(sellAssetRequestDto.Amount * currentAssetPrice),
                AvgCost = currentAssetPrice // Varlığın satış fiyatı
            };

            await _holdingService.UpsertHoldingAsync(userId, holdingDto);
            decimal totalCost = currentAssetPrice * sellAssetRequestDto.Amount;

            await _holdingService.UpdateCashHoldingAsync(userId, totalCost);

            await _transactionWriteRepository.SaveAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Varlik satimi sirasinda beklenmeyen bir hata meydana geldi.");
        }

        _logger.LogInformation(
            $"Degerli varlik Satildi : {_httpContextAccessor.HttpContext!.TraceIdentifier} {sellAssetRequestDto.Symbol} {sellAssetRequestDto.Amount} {currentAssetPrice}");
        return Result.Success("Varlik alim islemi basari ile tamamlandi.");
    }

    public async Task<Result> CreateBuyTransaction(string userId, BuyAssetRequestDto buyAssetRequestDto,
        decimal desiredAssetCost)
    {
        try
        {
            var assetToBuySymbol =
                await _symbolService.CheckIfSymbolExists(buyAssetRequestDto.AssetClass, buyAssetRequestDto.Symbol);
            if (assetToBuySymbol is null)
                throw new Exception("Islem yapmak istediginiz degerli varlik mevcut degil.");
            var transaction = new Transaction()
            {
                UserId = Guid.Parse(userId!),
                ExecutedAt = DateTime.UtcNow,
                SymbolId = Guid.Parse(assetToBuySymbol.SymbolId),
                Id = Guid.NewGuid(),
                Quantity = buyAssetRequestDto.Amount,
                Price = desiredAssetCost,
                Type = "BUY",
            };
            var res = await _transactionWriteRepository.AddAsync(transaction);
            var cashSymbol = await _symbolService.CheckIfSymbolExists("DEPOSIT", "USDT");
            if (cashSymbol is null)
                throw new InvalidOperationException("Nakit sembolu ('USDT') tanimli degil.");
            var cashTransferTransaction = new Transaction()
            {
                UserId = Guid.Parse(userId),
                ExecutedAt = DateTime.UtcNow,
                SymbolId = Guid.Parse(cashSymbol.SymbolId),
                Id = Guid.NewGuid(),
                Quantity = desiredAssetCost * buyAssetRequestDto.Amount,
                Price = 1,
                Type = $"TRANSFERRED_TO_BUY:{assetToBuySymbol.Code}",
            };
            await _transactionWriteRepository.AddAsync(cashTransferTransaction);
            var holdingDto = new CreateHoldingDto()
            {
                SymbolId = assetToBuySymbol.SymbolId,
                Quantity = buyAssetRequestDto.Amount,
                NetWorth = desiredAssetCost * buyAssetRequestDto.Amount,
                AvgCost = desiredAssetCost
            };

            await _holdingService.UpsertHoldingAsync(userId, holdingDto);
            decimal totalCost = desiredAssetCost * buyAssetRequestDto.Amount;
            await _holdingService.UpdateCashHoldingAsync(userId, -totalCost);

            await _transactionWriteRepository.SaveAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Varlik alimi sirasinda beklenmeyen bir hata meydana geldi.");
        }

        _logger.LogInformation(
            $"Degerli varlik satin alindi: {_httpContextAccessor.HttpContext!.TraceIdentifier} {buyAssetRequestDto.Symbol} {buyAssetRequestDto.Amount} {desiredAssetCost}");
        return Result.Success("Varlik alim islemi basari ile tamamlandi.");
    }
}