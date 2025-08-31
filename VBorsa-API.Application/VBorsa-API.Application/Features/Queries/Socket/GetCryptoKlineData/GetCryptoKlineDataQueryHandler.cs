using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using VBorsa_API.Application.DTOs.Helper;
using VBorsa_API.Application.DTOs.Quote.Binance;
using VBorsa_API.Application.Exceptions;
using VBorsa_API.Application.Features.Queries.Socket.GetCryptoData;

namespace VBorsa_API.Application.Features.Queries.Socket.GetCryptoKlineData;

public class
    GetCryptoKlineDataQueryHandler : IRequestHandler<GetCryptoKlineDataQueryRequest, Result<List<List<KlineDataDto>>>>
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://fapi.binance.com/fapi/v1/klines";
    private ILogger<GetCryptoKlineDataQueryHandler> _logger;

    public GetCryptoKlineDataQueryHandler(HttpClient httpClient, ILogger<GetCryptoKlineDataQueryHandler> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Result<List<List<KlineDataDto>>>> Handle(
        GetCryptoKlineDataQueryRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Symbol) || string.IsNullOrWhiteSpace(request.Interval))
            throw new ArgumentException("Sembol ve Aralik gereklidir.");

        var url = $"{BaseUrl}?symbol={request.Symbol}&interval={request.Interval}"
                  + (request.StartTime > 0 ? $"&startTime={request.StartTime * 1000}" : "")
                  + (request.EndTime > 0 ? $"&endTime={request.EndTime * 1000}" : "")
                  + (request.Limit is > 0 ? $"&limit={request.Limit}" : "");

        var json = await _httpClient.GetStringAsync(url, cancellationToken);
        var opts = new JsonSerializerOptions
            { NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString };
        var raw = JsonSerializer.Deserialize<List<List<JsonElement>>>(json, opts);
        if (raw is null || raw.Count == 0)
        {
            _logger.LogCritical("Binance Web Socket tarafında beklenmeyen bir hata meydana geldi," +
                                " müşteri temsilcisi ile iletişime geçiniz");
            throw new BinanceSocketException("Beklenmeyen bir hata meydana geldi.");
        }

        var klines = new List<KlineDataDto>(raw.Count);
        foreach (var k in raw)
        {
            if (k.Count < 7) continue;
            var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

            long openMs = AsLong(k[0]);
            long closeMs = AsLong(k[6]);

            var utcOpenTime = DateTimeOffset.FromUnixTimeMilliseconds(openMs).UtcDateTime;
            var turkeyOpenTime = TimeZoneInfo.ConvertTimeFromUtc(utcOpenTime, turkeyTimeZone);

            Console.WriteLine("openMS" + openMs);
            Console.WriteLine("closeMS" + closeMs);

            klines.Add(new KlineDataDto
            {
                EventTime = openMs,
                Symbol = request.Symbol.ToUpperInvariant(),
                OpenTime = turkeyOpenTime,
                CloseTime = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTimeOffset.FromUnixTimeMilliseconds(closeMs).UtcDateTime,
                    turkeyTimeZone),
                OpenPrice = AsDecimal(k[1]),
                HighPrice = AsDecimal(k[2]),
                LowPrice = AsDecimal(k[3]),
                ClosePrice = AsDecimal(k[4]),
                Volume = AsDecimal(k[5]),
                QuoteAssetVolume = k.Count > 7 ? AsDecimal(k[7]) : 0m,
                NumberOfTrades = k.Count > 8 ? (int)AsLong(k[8]) : 0,
                IsClosed = true
            });
        }

        var dto = new List<List<KlineDataDto>>
        {
            klines,
        };
        return Result<List<List<KlineDataDto>>>.Success(dto, "Kripto tablosu guncellendi");
    }

    private static long AsLong(JsonElement el)
    {
        return el.ValueKind switch
        {
            JsonValueKind.Number => el.GetInt64(),
            JsonValueKind.String => long.Parse(el.GetString()!, CultureInfo.InvariantCulture),
            _ => long.Parse(el.ToString(), CultureInfo.InvariantCulture)
        };
    }

    private static decimal AsDecimal(JsonElement el)
    {
        return el.ValueKind switch
        {
            JsonValueKind.Number => el.GetDecimal(),
            JsonValueKind.String => decimal.Parse(el.GetString()!, CultureInfo.InvariantCulture),
            _ => decimal.Parse(el.ToString(), CultureInfo.InvariantCulture)
        };
    }
}