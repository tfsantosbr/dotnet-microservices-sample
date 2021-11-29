using Microsoft.AspNetCore.Mvc;
using Users.Idp.Models;

namespace Users.Idp.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult Create([FromBody] CreateAccount request)
    {
        var userId = Guid.NewGuid();

        return Created($"account/{userId}", request);
    }

    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Delete(Guid userId)
    {
        return NoContent();
    }
}
