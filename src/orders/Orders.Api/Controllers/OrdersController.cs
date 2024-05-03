using System.Text.Json;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Orders.Api.Metrics;
using Orders.Api.Models;
using Orders.Api.Repositories;

namespace Orders.Api.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;
    private readonly IConfiguration _configuration;
    private readonly OrderRepository _repository;
    private readonly OrdersApiMetrics _metrics;

    public OrdersController(
        ILogger<OrdersController> logger,
        IConfiguration configuration,
        OrderRepository repository,
        OrdersApiMetrics metrics)
    {
        _logger = logger;
        _configuration = configuration;
        _repository = repository;
        _metrics = metrics;
    }

    // Public Methods

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Create([FromBody] OrderModel request)
    {
        var createOrderMessage = ConvertToCreateOrderToMessage(request);

        await SendCreateOrderMessage(createOrderMessage);

        return Accepted();
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrder(Guid orderId)
    {
        var order = await _repository.GetAsync(orderId);

        if (order is null)
        {
            return NotFound("order not found");
        }

        return Ok(order);
    }

    [HttpPut("{orderId}/confirm")]
    public async Task<IActionResult> ConfirmOrder(Guid orderId)
    {
        var order = await _repository.GetAsync(orderId);

        if (order is null)
        {
            return NotFound("order not found");
        }

        order.Confirm();

        await _repository.UpdateAsync(order);

        var durationInSeconds = GetConfirmationDurationMetric(order);

        _logger.LogInformation("Order '{orderId}' confirmed. Duration: {durationInSeconds} seconds.", order.OrderId, durationInSeconds);

        _metrics.RecordOrderConfirmationDuration(durationInSeconds);

        return NoContent();
    }

    // Private Methods

    private static double GetConfirmationDurationMetric(Order order)
    {
        if (!order.ConfirmedAt.HasValue)
            return 0;

        return (order.ConfirmedAt - order.CreatedAt).Value.TotalSeconds;
    }

    private static string ConvertToCreateOrderToMessage(OrderModel request)
    {
        return JsonSerializer.Serialize(request);
    }

    private async Task SendCreateOrderMessage(string message)
    {
        var topic = _configuration["Kafka:Topics:CreateOrderTopic"];
        var bootstrapServers = _configuration["Kafka:BootstrapServers"];

        _logger.LogInformation($"Conectando ao Kafka: {bootstrapServers}");

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
        };

        using var producer = new ProducerBuilder<Null, string>(config).Build();

        var result = await producer.ProduceAsync(
                topic,
                new Message<Null, string>
                { Value = message });

        _logger.LogInformation($"[Menssage Sent] \n" +
            $"Topic: {topic} \n" +
            $"Message: {message} \n" +
            $"Status: {result.Status}");
    }
}
