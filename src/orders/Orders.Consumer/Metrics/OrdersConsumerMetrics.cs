using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Orders.Consumer.Metrics;

public class OrdersConsumerMetrics : IDisposable
{
    internal const string ActivitySourceName = "Orders.Consumer.Metrics";
    internal const string MeterName = "Orders.Consumer.Metrics";
    private readonly Meter Meter;
    public ActivitySource ActivitySource { get; }

    public OrdersConsumerMetrics()
    {
        string? version = typeof(OrdersConsumerMetrics).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        Meter = new Meter(MeterName, version);

        OrderCreatrionDuration = Meter.CreateHistogram<double>(name: "orders.creation.duration", unit: "milliseconds" , description: "Duration of orders creation in milliseconds");
        OrderPrice = Meter.CreateHistogram<double>(name: "orders.price", description: "Price of orders");
    }

    // Properties

    private Histogram<double> OrderCreatrionDuration { get; }
    private Histogram<double> OrderPrice { get; }

    // Public Methods

    public void RecordOrderCreationDuration(double duration) => OrderCreatrionDuration.Record(duration);
    public void RecordOrderPrice(double price) => OrderPrice.Record(price);

    public void Dispose()
    {
        Meter.Dispose();
        ActivitySource.Dispose();
        GC.SuppressFinalize(this);
    }
}
