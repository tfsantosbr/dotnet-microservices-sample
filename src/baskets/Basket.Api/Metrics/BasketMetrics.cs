using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Basket.Api.Metrics;

public class BasketMetrics : IDisposable
{
    internal const string ActivitySourceName = "Basket.Api.Metrics.BasketMetrics";
    internal const string MeterName = "Basket.Api.Metrics.BasketMetrics";
    private readonly Meter Meter;
    public ActivitySource ActivitySource { get; }

    public BasketMetrics()
    {
        string? version = typeof(BasketMetrics).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        Meter = new Meter(MeterName, version);

        TotalBasketsCreated = Meter.CreateCounter<int>(name: "baskets.created.total", description: "Total number of created baskets");
        TotalBasketsRemoved = Meter.CreateCounter<int>(name: "baskets.removed.total", description: "Total number of removed baskets ");
        TotalProductsByBasket = Meter.CreateHistogram<int>(name: "baskets.products.quantity", description: "Quantity of products by basket");
    }

    // Properties

    private Counter<int> TotalBasketsCreated { get; }
    private Counter<int> TotalBasketsRemoved { get; }
    private Histogram<int> TotalProductsByBasket { get; }

    // Public Methods

    public void AddBasket() => TotalBasketsCreated.Add(1);
    public void RemoveBasket() => TotalBasketsRemoved.Add(1);
    public void RecordProductsByBasket(int productsQuantity) => TotalProductsByBasket.Record(productsQuantity);

    public void Dispose()
    {
        ActivitySource.Dispose();
        Meter.Dispose();

        GC.SuppressFinalize(this);
    }
}
