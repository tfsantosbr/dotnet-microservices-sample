using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Orders.Reporting.Metrics;

public class OrdersReportMetrics : IDisposable
{
    internal const string ActivitySourceName = "Orders.Reporting.Metrics";
    internal const string MeterName = "Orders.Reporting.Metrics";
    private readonly Meter Meter;
    public ActivitySource ActivitySource { get; }

    public OrdersReportMetrics()
    {
        string? version = typeof(OrdersReportMetrics).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        Meter = new Meter(MeterName, version);

        // Syncronous Metrics
        OrderConfirmationDuration = Meter.CreateHistogram<double>(name: "orders.confirmation.duration", unit: "seconds", description: "Duration of order confirmation in minutes");

        // Assincronous Metrics
        Meter.CreateObservableGauge(name: "orders.pending.count", observeValue: () => OrdersPendingCount, description: "Number of orders with pending confirmation");
        Meter.CreateObservableGauge(name: "orders.confirmed.count", observeValue: () => ConfirmedOrdersCount, description: "Number of confirmed orders");
        Meter.CreateObservableGauge(name: "orders.total.count", observeValue: () => TotalOrdersCount, description: "Total of orders");
    }

    // Properties

    private Histogram<double> OrderConfirmationDuration { get; }
    public long OrdersPendingCount { get; private set; }
    public long TotalOrdersCount { get; private set; }
    public long ConfirmedOrdersCount { get; private set; }

    // Public Methods

    public void RecordOrderConfirmationDuration(double durationInSeconds) => OrderConfirmationDuration.Record(durationInSeconds);
    public void UpdateOrdersPendingCount(long count) => OrdersPendingCount = count;
    public void UpdateTotalOrdersCount(long count) => TotalOrdersCount = count;
    public void UpdateConfirmedOrdersCount(long count) => ConfirmedOrdersCount = count;

    public void Dispose()
    {
        Meter.Dispose();
        ActivitySource.Dispose();
        GC.SuppressFinalize(this);
    }
}
