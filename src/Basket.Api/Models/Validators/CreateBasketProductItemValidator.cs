using FluentValidation;

namespace Basket.Api.Models.Validators
{
    public class CreateBasketProductItemValidator : AbstractValidator<CreateBasketProductItem>
    {
        public CreateBasketProductItemValidator()
        {
            RuleFor(p => p.ProductId).NotEmpty();
            RuleFor(p => p.ProductName).NotEmpty().Length(1, 500);
            RuleFor(p => p.Quantity).NotEmpty().InclusiveBetween(1, 100);
        }
    }
}