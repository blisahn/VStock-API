using MediatR;
using VBorsa_API.Application.DTOs.Quote.Binance;

// using VBorsa_API.Application.DTOs.Socket;

namespace VBorsa_API.Application.Features.Notifications.Binance;

public class BinanceDepthDataUpdateNotification : INotification
{
    public OrderBookDto? Data { get; set; }
}