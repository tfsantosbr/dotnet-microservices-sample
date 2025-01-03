using System.Text.Json;
using Basket.Api.Metrics;
using Basket.Api.Models;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Basket.Api.Controllers;

[ApiController]
[Route("baskets")]
public class BasketsController : ControllerBase
{
    private readonly ILogger<BasketsController> _logger;
    private readonly IConnectionMultiplexer _redis;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly BasketMetrics _metrics;

    public BasketsController(
        ILogger<BasketsController> logger,
        IConnectionMultiplexer redis,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        BasketMetrics metrics)
    {
        _logger = logger;
        _redis = redis;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _metrics = metrics;
    }

    // Public Methods

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateOrUpdate([FromBody] BasketModel request)
    {
        _logger.LogInformation("1. CONTROLLER: CreateOrUpdate {@request}", request);

        if (request.Products is null || request.ProductsTotalQuantity == 0)
            return BadRequest("Products cannot be null");

        request.User = await GetUserDetails(request.UserId);

        if (request.User == null) return NotFound("User not found");

        var existingBasket = await GetBasket(request.User.Id);

        if (existingBasket != null)
        {
            existingBasket = request;
            await SaveBasket(existingBasket);
        }

        var newBasket = request;

        await SaveBasket(newBasket);

        _logger.LogInformation("6. CONTROLLER: Basket created or updated {@request}", request);

        // metrics

        _metrics.AddBasket(newBasket.City);
        _metrics.RecordProductsByBasket(request.ProductsTotalQuantity, newBasket.City);

        return Ok(request);
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(Guid userId)
    {
        _logger.LogInformation("Getting user basket {UserId}", userId);

        var basket = await GetBasket(userId);

        if (basket == null)
            return NotFound();

        return Ok(basket);
    }

    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid userId)
    {
        _logger.LogInformation("Deleting user basket {UserId}", userId);

        await RemoveBasket(userId);

        _metrics.RemoveBasket();

        return NoContent();
    }

    // Private methods

    private async Task<UserModel?> GetUserDetails(Guid? userId)
    {
        _logger.LogInformation("2. PRIVATE METHOD: GetUserDetails: {userId}", userId);

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_configuration["UsersApiEndpoint"]!);

        var response = await client.GetAsync($"account/{userId}");

        response.EnsureSuccessStatusCode();

        var userString = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<UserModel>(userString);
    }

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
