using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Basket.Api.Models
{
    public class BasketModel
    {
        public Guid? UserId { get; set; }
        public IEnumerable<BasketProductItemModel>? Products { get; set; }
    }
}