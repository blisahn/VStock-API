using MediatR;
using VBorsa_API.Application.DTOs.Quote.Binance;

namespace VBorsa_API.Application.Features.Notifications.Binance;

public class  BinanceTradeDataUpdateNotification : INotification
{
    public TradeDto? Data { get; set; }
}