using System.Diagnostics;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Products.Api.Metrics;

namespace Products.Api.Controllers;

[ApiController]
[Route("products/import")]
public class ProductsImportController : ControllerBase
{
    private readonly ILogger<ProductsImportController> _logger;
    private readonly ProductsApiMetrics _metrics;

    public ProductsImportController(ILogger<ProductsImportController> logger, ProductsApiMetrics metrics)
    {
        _logger = logger;
        _metrics = metrics;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Import([FromBody] int TotalProducts, CancellationToken cancelationToken)
    {
        var task = Task.Run(async () =>
        {
            var importId = Guid.NewGuid().ToString()[..8];
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            _logger.LogInformation("[PRODUCTS IMPORT] Importing ID: {importId} -> {TotalProducts} products...",
                importId, TotalProducts);

            _metrics.IncreaseActiveImports();

            for (int i = 0; i < TotalProducts; i++)
            {
                // simulate a product import
                await Task.Delay(500);

                _logger.LogInformation("[PRODUCTS IMPORT] Imported product {ProductNumber}", i + 1);
            }

            stopwatch.Stop();

            _logger.LogInformation("[PRODUCTS IMPORT] Import ID: {importId} finished. "
                + "Imported {TotalProducts} products in {ElapsedSeconds}s",
                importId,
                TotalProducts,
                stopwatch.Elapsed.TotalSeconds);

            _metrics.DecreaseActiveImports();
            _metrics.RecordImportProductsDucation(stopwatch.Elapsed.TotalSeconds);

        }, cancelationToken);

        return Accepted();
    }
}
