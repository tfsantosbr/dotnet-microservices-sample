using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Products.Api.Metrics;

public class ProductsApiMetrics : IDisposable
{
    internal const string ActivitySourceName = "Products.Api.Metrics";
    internal const string MeterName = "Products.Api.Metrics";
    private readonly Meter Meter;
    public ActivitySource ActivitySource { get; }

    public ProductsApiMetrics()
    {
        string? version = typeof(ProductsApiMetrics).Assembly.GetName().Version?.ToString();
        ActivitySource = new ActivitySource(ActivitySourceName, version);
        Meter = new Meter(MeterName, version);

        ActiveImportsCount = Meter.CreateUpDownCounter<int>("products.imports.active.count");
        ImportProductsDucation = Meter.CreateHistogram<double>("products.import.duration", "seconds");

        Meter.creat
    }

    // Properties

    private UpDownCounter<int> ActiveImportsCount { get; }
    private Histogram<double> ImportProductsDucation { get; }

    // Public Methods

    public void IncreaseActiveImports() => ActiveImportsCount.Add(1);
    public void DecreaseActiveImports() => ActiveImportsCount.Add(-1);
    public void RecordImportProductsDucation(double duration) => ImportProductsDucation.Record(duration);

    public void Dispose()
    {
        ActivitySource.Dispose();
        Meter.Dispose();

        GC.SuppressFinalize(this);
    }
}
