using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Basket.Api.Models
{
    public class CreateBasketProductItem
    {
        public Guid? ProductId { get; set; }

        public string? ProductName { get; set; }

        public int? Quantity { get; set; }
    }
}