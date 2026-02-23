using RealTimeOrder.Api.Model;
using RealTimeOrder.Api.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<OrderStreamService>();

var app = builder.Build();

app.MapGet("/orders/{orderId:guid}/stream", (
    Guid orderId,
    OrderStreamService streamService,
    CancellationToken cancellationToken) =>
{
    var reader = streamService.Subscribe(orderId);

    return Results.ServerSentEvents(
        reader.ReadAllAsync(cancellationToken),
        eventType: "order-update");
});

app.MapPost("/orders/{orderId:guid}/simulate", async (
    Guid orderId,
    OrderStreamService streamService) =>
{
    var statuses = Enum.GetValues<OrderStatus>();

    foreach (var status in statuses)
    {
        await Task.Delay(2000);

        await streamService.PublishAsync(new OrderStatusUpdate(
            orderId,
            status,
            DateTime.UtcNow));
    }

    return Results.Ok("Order simulation completed.");
});

app.Run();