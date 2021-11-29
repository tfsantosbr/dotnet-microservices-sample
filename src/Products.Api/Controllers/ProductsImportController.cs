using System.Diagnostics;
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

        var endTime = DateTime.UtcNow.AddMinutes(1);

        try
        {
            while (DateTime.UtcNow <= endTime)
            {
                cancelationToken.ThrowIfCancellationRequested();
                Console.WriteLine($"Time: {DateTime.UtcNow:mm:ss.fff}");
            }
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
