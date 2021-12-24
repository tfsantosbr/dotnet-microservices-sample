namespace Users.Idp.Domain
{
    public class User
    {
        public User(string name, string email, string password, Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            Name = name;
            Email = email;
            Password = password;
        }

        private User()
        {
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string Password { get; private set; } = null!;
    }
}