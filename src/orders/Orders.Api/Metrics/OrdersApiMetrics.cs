using System.Diagnostics;
using System.Diagnostics.Metrics;
using Orders.Api.Repositories;

namespace Orders.Api.Metrics;

public class OrdersApiMetrics : IDisposable
{
    internal const string ActivitySourceName = "Orders.Api.Metrics";
    internal const string MeterName = "Orders.Api.Metrics";
    private readonly Meter Meter;
    public ActivitySource ActivitySource { get; }

    public OrdersApiMetrics()
    {
        string? version = typeof(OrdersApiMetrics).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        Meter = new Meter(MeterName, version);

        OrderConfirmationDuration = Meter.CreateHistogram<double>(name: "orders.confirmation.duration", unit: "seconds", description: "Duration of order confirmation in minutes");
    }

    // Properties

    private Histogram<double> OrderConfirmationDuration { get; }

    // Public Methods

    public void RecordOrderConfirmationDuration(double durationInSeconds) => OrderConfirmationDuration.Record(durationInSeconds);

    public void Dispose()
    {
        Meter.Dispose();
        ActivitySource.Dispose();
        GC.SuppressFinalize(this);
    }
}
