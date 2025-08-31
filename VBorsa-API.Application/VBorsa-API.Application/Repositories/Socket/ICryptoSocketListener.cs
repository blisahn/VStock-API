using VBorsa_API.Application.DTOs.Quote;
using VBorsa_API.Application.DTOs.Quote.Binance;
using VBorsa_API.Application.DTOs.Socket;

namespace VBorsa_API.Application.Repositories.Socket;

public interface ICryptoSocketListener
{
    Task ProcessFinHubTradeData(FinhubTradeDataDto finHubTradeData);
    Task ProcessKlineData(KlineDataDto klineData);
    Task ProcessDepthData(OrderBookDto orderBookDto);
    Task ProcessTradeData(TradeDto tradeData);
}