using FluentValidation;

namespace Basket.Api.Models.Validators
{
    public class CreateBasketValidator : AbstractValidator<CreateBasket>
    {
        public CreateBasketValidator()
        {
            RuleFor(p => p.UserId).NotEmpty();
            RuleFor(p => p.Products).NotEmpty();
            RuleForEach(p => p.Products).SetValidator(new CreateBasketProductItemValidator());
        }
    }
}