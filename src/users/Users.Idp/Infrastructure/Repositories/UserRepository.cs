using Users.Idp.Domain;

namespace Users.Idp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ILogger<UserRepository> logger)
    {
        _logger = logger;
    }

    public async Task<User> GetUserAsync(Guid userId)
    {
        _logger.LogInformation("5. USER REPOSITORY: GetUserAsync: {userId}", userId);

        // if (userId == new Guid("37a528bd-7828-4636-89f2-42971e8fc568"))
        // {
            throw new ApplicationException($"CUSTOM ERROR: An error occurred when trying " + 
                $"to search for the user with the id '{userId}'.");
        // }

        // var random = new Random();
        // int randomNumber = random.Next(100);

        // if (randomNumber < 10)
        // {
        //     throw new ApplicationException($"Custom Error: User with id '{id}' not found.");
        // }

        var fakeUser = new User(
            Faker.Name.FullName(),
            Faker.Internet.Email(),
            Faker.Identification.SocialSecurityNumber()
        );

        return await Task.FromResult(fakeUser);
    }
}