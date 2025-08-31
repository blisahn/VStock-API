using MediatR;
using VBorsa_API.Application.DTOs.Quote.Binance;

namespace VBorsa_API.Application.Features.Notifications.Binance;

public class BinancePublicMarketDataUpdateNotification: INotification
{
    public TradeDto? Data { get; set; }

}