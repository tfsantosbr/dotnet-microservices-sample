using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Products.Api.Controllers;

[ApiController]
[Route("products/images/import")]
public class ProductsImagesImportController : ControllerBase
{
    private readonly ILogger<ProductsImagesImportController> _logger;

    public ProductsImagesImportController(ILogger<ProductsImagesImportController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Import()
    {
        var bigString = new String('x', 100000 * 1024);

        return Ok();
    }
}
