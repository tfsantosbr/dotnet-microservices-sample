namespace Users.Idp.Domain;

public interface IUserRepository
{
    Task<User> GetUserAsync(Guid id);
}
