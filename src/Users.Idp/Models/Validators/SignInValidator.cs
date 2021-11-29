using FluentValidation;

namespace Users.Idp.Models.Validators
{
    public class SignInValidator : AbstractValidator<SignIn>
    {
        public SignInValidator()
        {
            RuleFor(p => p.Email).NotEmpty().EmailAddress();
            RuleFor(p => p.Password).NotEmpty();
        }
    }
}