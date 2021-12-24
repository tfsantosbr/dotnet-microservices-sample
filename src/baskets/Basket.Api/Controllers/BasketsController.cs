using System.Text.Json;
using System.Text.Json.Serialization;
using Basket.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.Api.Controllers;

[ApiController]
[Route("baskets")]
public class BasketsController : ControllerBase
{
    private readonly ILogger<BasketsController> _logger;
    private readonly IDistributedCache _cache;

    public BasketsController(ILogger<BasketsController> logger, IDistributedCache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    // Public Methods

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOrUpdate([FromBody] BasketModel request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;

        var existingBasket = await GetBasket(userId, cancellationToken);

        if (existingBasket != null)
        {
            existingBasket = request;
            await SaveBasket(existingBasket, cancellationToken);
        }

        var newBasket = request;
        await SaveBasket(newBasket, cancellationToken);

        return Ok(request);
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid userId, CancellationToken cancellationToken)
    {
        var basket = await GetBasket(userId, cancellationToken);

        if (basket == null)
            return NotFound();

        return Ok(basket);
    }

    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid userId, CancellationToken cancellationToken)
    {
        await RemoveBasket(userId, cancellationToken);

        return NoContent();
    }

    // Private methods

    private Task RemoveBasket(Guid userId, CancellationToken cancellationToken)
    {
        return _cache.RemoveAsync(userId.ToString(), cancellationToken);
    }

    private async Task<BasketModel?> GetBasket(Guid? userId, CancellationToken cancellationToken)
    {
        var basketString = await _cache.GetStringAsync(userId.ToString(), cancellationToken);

        if (basketString == null) return null;

        return JsonSerializer.Deserialize<BasketModel>(basketString);
    }

    private async Task SaveBasket(BasketModel basket, CancellationToken cancellationToken)
    {
        var basketString = JsonSerializer.Serialize(basket);

        await _cache.SetStringAsync(basket.UserId.ToString(), basketString, cancellationToken);
    }
}
