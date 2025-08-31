using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.Socket.Data;
using VBorsa_API.Application.DTOs.Symbol;
using VBorsa_API.Application.DTOs.Transaction;

namespace VBorsa_API.Application.Abstractions.Services.Symbol;

public interface ISymbolService
{
    Task<Result> CreateAssetSymbolAsync(CreateSymbolRequestDto symbolRequestDto);
    Task<List<SymbolDto>> GetByClassType(string requestAssetClass);
    Task<AssetSymbolDetailsDto> GetAssetSymbolDetailsById(string requestId);
    Task<Result> UpdateSymbolAsync(string dtoSymbolId, UpdateSymbolDto dto);
    Task<IEnumerable<string>> GetActiveSymbolsAsync();
    Task<IEnumerable<string>> GetPubliclyVisibleSymbolsAsync();
   
    Task<SymbolDto?> CheckIfSymbolExists(string assetClass, string symbolCode);
    Task<Result>  CreateDepositSymbolDto(CreateDepositDto createDepositDto);
}