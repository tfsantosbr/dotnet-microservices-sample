using Orders.Reporting.Metrics;
using Orders.Reporting.Repositories;

namespace Orders.Reporting;

public class OrdersReportingWorker : BackgroundService
{
    private readonly OrderRepository _repository;
    private readonly OrdersReportMetrics _metrics;
    private ILogger<OrdersReportingWorker> _logger;

    public OrdersReportingWorker(
        OrderRepository repository, OrdersReportMetrics metrics, 
        ILogger<OrdersReportingWorker> logger)
    {
        _repository = repository;
        _metrics = metrics;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("[REPORTING ORDERS] Processing count of orders...");

            var pendingOrdersCount = await _repository.CountPendingOrders();
            var confirmedOrdersCount = await _repository.CountConfirmedOrders();
            var totalOrdersCount = await _repository.CountTotalOrders();

            _metrics.UpdateOrdersPendingCount(pendingOrdersCount);
            _metrics.UpdateConfirmedOrdersCount(confirmedOrdersCount);
            _metrics.UpdateTotalOrdersCount(totalOrdersCount);

            _logger.LogInformation("[ORDERS REPORTED] Orders report => Pending: {pending} | "
                + "Confirmed: {confirmed} | Total: {total}", 
                pendingOrdersCount, confirmedOrdersCount, totalOrdersCount);

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}