using Microsoft.AspNetCore.Mvc;
using Users.Idp.Domain;
using Users.Idp.Infrastructure.Context;
using Users.Idp.Models;

namespace Users.Idp.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IUserService _userService;
    private readonly UsersDbContext _context;

    public AccountController(ILogger<AccountController> logger, UsersDbContext context, IUserService userService)
    {
        _logger = logger;
        _context = context;
        _userService = userService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateAccount request)
    {
        _logger.LogInformation("Creating account for {Name}", request.Name);

        var user = new User(request.Name, request.Email, request.Password);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return Created($"account/{user.Id}", user);
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        _logger.LogInformation("3. CONTROLLER: GetUser: {userId}", userId);

        var fakeUser = await _userService.GetUserAsync(userId);

        return Ok(fakeUser);
    }

    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Delete(Guid userId)
    {
        _logger.LogInformation("Deleting user {UserId}", userId);
        return NoContent();
    }
}
