using Microsoft.AspNetCore.Mvc;

namespace Products.Api.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ILogger<ProductsController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult Create()
    {
        var productId = Guid.NewGuid();

        return Created($"products/{productId}", new { productId });
    }

    [HttpPut("{productId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Update(Guid productId)
    {
        return NoContent();
    }

    [HttpDelete("{productId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Delete(Guid productId)
    {
        throw new NotImplementedException();
    }
}
