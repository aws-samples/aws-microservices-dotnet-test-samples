using System.ComponentModel.DataAnnotations;

namespace ShoppingCartService.Controllers.Models;

public record ShippingAddressDto(
    [Required] string Name,
    [Required] string Country,
    [Required] string City,
    [Required] string Street);