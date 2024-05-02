using System.Text.Json;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Orders.Api.Models;

namespace Orders.Api.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;
    private IConfiguration _configuration;

    public OrdersController(ILogger<OrdersController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
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

    [HttpPut("{orderId}/confirm")]
    public IActionResult ConfirmOrder(Guid orderId)
    {
        var order = GetOrder(orderId);
        
        order.Confirm();

        return Ok(order);
    }

    private static OrderModel GetOrder(Guid orderId)
    {
        var random = new Random();
        var randomDate = random.Next(1, 7);

        OrderModel order = new()
        {
            OrderId = orderId,
            UserId = Guid.NewGuid(),
            CreatedAt = DateTime.Now.AddDays(-randomDate),
        };

        return order;
    }

    // Private Methods

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
