using Confluent.Kafka;

namespace Orders.Consumer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string topic = _configuration["Kafka:Topics:CreateOrderTopic"];
        string groupId = _configuration["Kafka:GroupId"];
        string bootstrapServers = _configuration["Kafka:BootstrapServers"];

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

                    _logger.LogInformation($"[ORDER CREATED]: '{result.Message.Value}'");
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

        await Task.CompletedTask;
    }
}
