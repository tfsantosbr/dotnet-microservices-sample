using Microsoft.AspNetCore.Mvc;

namespace Products.Api.Controllers;

[ApiController]
[Route("products/import")]
public class ProductsImportController : ControllerBase
{
    private readonly ILogger<ProductsImportController> _logger;

    public ProductsImportController(ILogger<ProductsImportController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Import(CancellationToken cancelationToken)
    {
        // code for consume CPU

        var endTime = DateTime.UtcNow.AddSeconds(3);

        try
        {
            _logger.LogInformation($"Start processing at: {DateTime.UtcNow:HH:mm:ss.fff}");

            while (DateTime.UtcNow <= endTime)
            {
                cancelationToken.ThrowIfCancellationRequested();
            }

            _logger.LogInformation($"Process terminated at: {DateTime.UtcNow:HH:mm:ss.fff}");
        }
        catch (OperationCanceledException exception)
        {
            _logger.LogInformation($"Products import operation is canceled: {exception.Message}");
        }
        finally
        {
            await Task.CompletedTask;
        }

        return Ok();
    }
}
