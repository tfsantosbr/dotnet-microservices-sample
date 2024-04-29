using Users.Idp.Domain;

namespace Users.Idp.Infrastructure.Repositories;

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