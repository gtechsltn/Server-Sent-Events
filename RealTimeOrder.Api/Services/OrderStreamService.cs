using RealTimeOrder.Api.Model;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace RealTimeOrder.Api.Services;

public sealed class OrderStreamService
{
    private readonly ConcurrentDictionary<Guid, Channel<OrderStatusUpdate>> _streams = new();

    public ChannelReader<OrderStatusUpdate> Subscribe(Guid orderId)
    {
        var channel = Channel.CreateUnbounded<OrderStatusUpdate>();
        _streams[orderId] = channel;
        return channel.Reader;
    }

    public async Task PublishAsync(OrderStatusUpdate update)
    {
        if (_streams.TryGetValue(update.OrderId, out var channel))
        {
            await channel.Writer.WriteAsync(update);
        }
    }

    public void Unsubscribe(Guid orderId)
    {
        if (_streams.TryRemove(orderId, out var channel))
        {
            channel.Writer.TryComplete();
        }
    }
}