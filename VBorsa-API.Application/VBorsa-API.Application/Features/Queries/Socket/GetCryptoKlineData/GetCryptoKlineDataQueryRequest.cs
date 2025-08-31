using MediatR;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.Quote.Binance;
using VBorsa_API.Application.DTOs.Socket.Data;

namespace VBorsa_API.Application.Features.Queries.Socket.GetCryptoData;

public class GetCryptoKlineDataQueryRequest : IRequest<Result<List<List<KlineDataDto>>>>
{
    public required string Symbol { get; set; }
    public required string Interval { get; set; }
    public long StartTime { get; set; }
    public long EndTime { get; set; }
    public int? Limit { get; set; }
}