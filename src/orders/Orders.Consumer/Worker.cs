using System.Text.Json;
using Confluent.Kafka;
using Orders.Consumer.Models;
using Orders.Consumer.Repositories;

namespace Orders.Consumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly OrderRepository _repository;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, OrderRepository repository)
    {
        _logger = logger;
        _configuration = configuration;
        _repository = repository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string topic = _configuration["Kafka:Topics:CreateOrderTopic"];
        string groupId = _configuration["Kafka:GroupId"];
        string bootstrapServers = _configuration["Kafka:BootstrapServers"];

        _logger.LogInformation($"Conectando ao Kafka: {bootstrapServers}");

        var consumerConfig = new ConsumerConfig
        {
            GroupId = groupId,
            BootstrapServers = bootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Latest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        consumer.Subscribe(topic);

        _logger.LogInformation("Consumer started...");
        _logger.LogInformation($"Topic: {topic}");
        _logger.LogInformation($"Group ID: {groupId}");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    var order = GetOrder(result.Message.Value);

                    if (order == null)
                        throw new NullReferenceException(nameof(order));

                    _logger.LogInformation($"[ORDER RECEIVED]: '{order.OrderId}' | Products Count: {order.Products?.Count()}");

                    await _repository.CreateAsync(order);

                    _logger.LogInformation($"[ORDER SAVED]: '{order.OrderId}'");
                }
                catch (ConsumeException exception)
                {
                    _logger.LogError($"An error ocurred while consuming topic: {topic}", exception);

                    throw exception;
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Cancellation token requested. Canceling Operation...");
            consumer.Close();
        }
        catch (Exception exception)
        {
            throw exception;
        }

        await Task.CompletedTask;
    }

    private Order? GetOrder(string value)
    {
        return JsonSerializer.Deserialize<Order>(value);
    }
}
