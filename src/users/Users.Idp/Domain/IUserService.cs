namespace Users.Idp.Domain;

public interface IUserService
{
    Task<User> GetUserAsync(Guid id);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> GetUserAsync(Guid id)
    {
        return await _userRepository.GetUserAsync(id);
    }
}

public interface IUserRepository
{
    Task<User> GetUserAsync(Guid id);
}

public class UserRepository : IUserRepository
{
    public async Task<User> GetUserAsync(Guid id)
    {
        var random = new Random();
        int randomNumber = random.Next(100);

        if (randomNumber < 30)
        {
            throw new ApplicationException($"Custom Error: User with id '{id}' not found.");
        }

        var fakeUser = new User(
            Faker.Name.FullName(),
            Faker.Internet.Email(),
            Faker.Identification.SocialSecurityNumber()
        );

        return await Task.FromResult(fakeUser);
    }
}