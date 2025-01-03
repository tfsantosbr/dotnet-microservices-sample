using Microsoft.AspNetCore.Mvc;
using Users.Idp.Models;

namespace Users.Idp.Controllers;

[ApiController]
[Route("login")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;

    public LoginController(ILogger<LoginController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult SignIn([FromBody] SignIn request)
    {
        _logger.LogInformation("Signing in user {Email}", request.Email);
        
        var tokenId = Guid.NewGuid();

        return Ok(new { tokenId });
    }
}
