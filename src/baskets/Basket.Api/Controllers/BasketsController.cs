using System.Text.Json;
using Basket.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Basket.Api.Controllers;

[ApiController]
[Route("baskets")]
public class BasketsController : ControllerBase
{
    private readonly ILogger<BasketsController> _logger;
    private readonly IConnectionMultiplexer _redis;
    private readonly IConfiguration _configuration;

    public BasketsController(
        ILogger<BasketsController> logger, IConnectionMultiplexer redis, IConfiguration configuration)
    {
        _logger = logger;
        _redis = redis;
        _configuration = configuration;
    }

    // Public Methods

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOrUpdate([FromBody] BasketModel request)
    {
        request.User = await GetUserDetails(request.UserId);

        if(request.User == null) return NotFound("User not found");

        var existingBasket = await GetBasket(request.User.Id);

        if (existingBasket != null)
        {
            existingBasket = request;
            await SaveBasket(existingBasket);
        }

        var newBasket = request;
        await SaveBasket(newBasket);

        return Ok(request);
    }

    private async Task<UserModel?> GetUserDetails(Guid? userId)
    {
        var client = new HttpClient();
        var endpoint = _configuration["UsersApiEndpoint"];
        var response = await client.GetAsync($"{endpoint}/{userId}");

        if (!response.IsSuccessStatusCode) return null;

        var userString = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<UserModel>(userString);
    }



    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid userId)
    {
        var basket = await GetBasket(userId);

        if (basket == null)
            return NotFound();

        return Ok(basket);
    }

    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid userId)
    {
        await RemoveBasket(userId);

        return NoContent();
    }

    // Private methods

    private async Task RemoveBasket(Guid userId)
    {
        var db = _redis.GetDatabase();
        await db.KeyDeleteAsync(userId.ToString());
    }

    private async Task<BasketModel?> GetBasket(Guid? userId)
    {
        var db = _redis.GetDatabase();
        var basketString = await db.StringGetAsync(userId.ToString());

        if (!basketString.HasValue) return null;

        return JsonSerializer.Deserialize<BasketModel>(basketString);
    }

    private async Task SaveBasket(BasketModel basket)
    {
        var db = _redis.GetDatabase();
        var basketString = JsonSerializer.Serialize(basket);

        await db.StringSetAsync(basket.UserId.ToString(), basketString);
    }
}
