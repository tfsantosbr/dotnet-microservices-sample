using Microsoft.AspNetCore.Mvc;
using Products.Api.Domain.Products;
using Products.Api.Domain.Products.Commands;
using Products.Api.Infrastructure.Context;

namespace Products.Api.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly ProductsDbContext _context;

    public ProductsController(ILogger<ProductsController> logger, ProductsDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateProduct request)
    {
        var product = new Product(
            name: request.Name,
            price: request.Price,
            quantity: request.Quantity
        );

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return Created($"products/{product.Id}", product);
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
