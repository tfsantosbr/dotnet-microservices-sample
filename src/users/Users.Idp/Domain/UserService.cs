namespace Users.Idp.Domain;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<User> GetUserAsync(Guid userId)
    {
        _logger.LogInformation("4. USER SERVICE: GetUserAsync: {userId}", userId);
        
        return await _userRepository.GetUserAsync(userId);
    }
}
