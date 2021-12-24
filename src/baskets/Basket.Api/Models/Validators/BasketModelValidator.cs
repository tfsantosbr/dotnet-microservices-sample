using FluentValidation;

namespace Basket.Api.Models.Validators
{
    public class BasketModelValidator : AbstractValidator<BasketModel>
    {
        public BasketModelValidator()
        {
            RuleFor(p => p.UserId).NotEmpty();
            RuleFor(p => p.Products).NotEmpty();
            RuleForEach(p => p.Products).SetValidator(new BasketProductItemModelValidator());
        }
    }
}