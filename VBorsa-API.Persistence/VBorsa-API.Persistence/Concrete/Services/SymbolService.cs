using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VBorsa_API.Application.Abstractions.Services.Symbol;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.Socket.Data;
using VBorsa_API.Application.DTOs.Symbol;
using VBorsa_API.Application.DTOs.Transaction;
using VBorsa_API.Application.Exceptions;
using VBorsa_API.Application.Repositories.Symbol;
using VBorsa_API.Core.Entities;

namespace VBorsa_API.Persistence.Concrete.Services;

public class SymbolService : ISymbolService
{
    private readonly ISymbolReadRepository _symbolReadRepository;
    private readonly ISymbolWriteRepository _symbolWriteRepository;
    private readonly ILogger<SymbolService> _logger;

    public SymbolService(ISymbolWriteRepository symbolWriteRepository, ISymbolReadRepository symbolReadRepository,
        ILogger<SymbolService> logger)
    {
        _symbolWriteRepository = symbolWriteRepository;
        _symbolReadRepository = symbolReadRepository;
        _logger = logger;
    }


    public async Task<Result> CreateAssetSymbolAsync(CreateSymbolRequestDto symbolRequestDto)
    {
        if (!symbolRequestDto.Code.Contains("USDT"))
        {
            symbolRequestDto.Code += "USDT";
        }

        var symbol = new Symbol()
        {
            AssetClass = symbolRequestDto.AssetClass,
            Code = symbolRequestDto.Code,
            Id = Guid.NewGuid(),
            Source = symbolRequestDto.Source,
            IsActive = symbolRequestDto.IsActive,
            IsVisibleForNonLogin = symbolRequestDto.IsVisibleForNonLogin,
        };
        var exists = _symbolReadRepository.GetWhere(s => s.Code == symbolRequestDto.Code).ToList();
        if (exists.Count != 0) throw new SymbolAlreadyExistsException("Sembol zaten sisteme kayitli");
        try
        {
            await _symbolWriteRepository.AddAsync(symbol);
            await _symbolWriteRepository.SaveAsync();
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Sembolun sisteme kayidi yapilirken bir hata meydana geldi");

            throw new SymbolCreationFailedExpceion("Sembol olustururken beklenmeyen bir hata ile karsilasildi");
        }

        _logger.LogInformation($"Yeni bir sembol:{symbol.Code} sisteme kayit edildi");
        return Result.Success("Sembol sisteme kayit edildi");
    }

    public async Task<List<SymbolDto>> GetByClassType(string requestAssetClass)
    {
        var symbols = _symbolReadRepository.GetAll().Where(s => s.AssetClass == requestAssetClass).ToList();
        if (symbols.Count == 0)
        {
            throw new UpdateException("Sistemde istenilen turde bir sembol bulunmamaktadir");
        }

        var results = new List<SymbolDto>(symbols.Count);

        foreach (var symbol in symbols)
        {
            results.Add(new SymbolDto()
            {
                SymbolId = symbol.Id.ToString(),
                AssetClass = symbol.AssetClass,
                Code = symbol.Code,
                Source = symbol.Code,
                IsActive = symbol.IsActive,
                IsVisibleForNonLogin = symbol.IsVisibleForNonLogin,
            });
        }

        return results;
    }

    public async Task<AssetSymbolDetailsDto> GetAssetSymbolDetailsById(string requestId)
    {
        var asset = await _symbolReadRepository.GetByIdAsync(requestId);
        var response = new AssetSymbolDetailsDto()
        {
            AssetClass = asset.AssetClass,
            Code = asset.Code,
            IsActive = asset.IsActive,
            Source = asset.Source
        };
        return response;
    }

    public async Task<Result> UpdateSymbolAsync(string dtoSymbolId, UpdateSymbolDto dto)
    {
        var symbol = await _symbolReadRepository.GetByIdAsync(dtoSymbolId!) ??
                     throw new SymbolNotExistsException("Sembol bulunamadi");
        int result = 0;
        symbol.Id = Guid.Parse(dto.Id);
        if (symbol.IsActive == dto.IsActive)
        {
        }
        else
        {
            symbol.IsActive = dto.IsActive;
            _symbolWriteRepository.Table.Entry(symbol).Property(x => x.IsActive).IsModified = true;
            result += await _symbolWriteRepository.SaveAsync();
        }

        if (symbol.IsVisibleForNonLogin == dto.IsVisibleForNonLogin)
        {
        }
        else
        {
            symbol.IsVisibleForNonLogin = dto.IsVisibleForNonLogin;
            _symbolWriteRepository.Table.Entry(symbol).Property(x => x.IsVisibleForNonLogin).IsModified = true;
            result += await _symbolWriteRepository.SaveAsync();
        }

        if (result <= 0)
        {
            _logger.LogError("Güncelleme için gerekli sorgrular yapıldı, ama hiçbir değişiklik olmadı");
            throw new UpdateException("Semboller güncellenirken bir hata ile karşılaşıldı");

        }

        Console.WriteLine("Yeni durum:" + _symbolReadRepository.GetByIdAsync(symbol.Id.ToString()).Result.IsActive);
        return Result.Success("Sembol  basari ile guncellenmistir.");
    }

    public async Task<IEnumerable<string>> GetActiveSymbolsAsync()
    {
        var res = await Task.FromResult<IEnumerable<string>>(_symbolReadRepository.Table
            .Where(s => s.IsActive && s.AssetClass != "DEPOSIT")
            .Select(s => s.Code));

        return res.ToList();
    }

    public async Task<IEnumerable<string>> GetPubliclyVisibleSymbolsAsync()
    {
        var res = await Task.FromResult<IEnumerable<string>>(_symbolReadRepository.Table
            .Where(s => s.IsVisibleForNonLogin)
            .Select(s => s.Code.ToUpperInvariant()));
        return res.ToList();
    }

    public async Task<SymbolDto?> CheckIfSymbolExists(string assetClass, string symbolCode)
    {
        var res = await _symbolReadRepository.Table
            .FirstOrDefaultAsync(s =>
                s.AssetClass == assetClass && s.Code == symbolCode);
        if (res is null) return null;
        return new SymbolDto()
        {
            AssetClass = res!.AssetClass!,
            Code = res!.Code,
            IsActive = res!.IsActive,
            Source = res!.Source,
            SymbolId = res!.Id.ToString(),
        };
    }

    public async Task<Result> CreateDepositSymbolDto(CreateDepositDto createDepositDto)
    {
        var res = await _symbolWriteRepository.AddAsync(new Symbol()
        {
            Id = createDepositDto.SymbolId,
            AssetClass = createDepositDto.TransactionType,
            Code = createDepositDto.Code,
            IsActive = false,
            Source = "USER",
            IsVisibleForNonLogin = false
        });
        await _symbolWriteRepository.SaveAsync();
        return Result.Success("Sembol  basari ile guncellenmistir.");
    }
}