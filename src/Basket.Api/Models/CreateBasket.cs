using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Basket.Api.Models
{
    public class CreateBasket
    {
        public Guid? UserId { get; set; }
        public IEnumerable<CreateBasketProductItem>? Products { get; set; }
    }
}