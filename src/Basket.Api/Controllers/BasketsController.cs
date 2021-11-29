using Basket.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Basket.Api.Controllers;

[ApiController]
[Route("baskets")]
public class BasketsController : ControllerBase
{
    private readonly ILogger<BasketsController> _logger;

    public BasketsController(ILogger<BasketsController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult CreateOrUpdate([FromBody] CreateBasket request)
    {
        var basketId = Guid.NewGuid();

        return Ok(request);
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get(Guid userId)
    {
        return Ok(new { userId });
    }

    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Delete(Guid userId)
    {
        return NoContent();
    }
}
