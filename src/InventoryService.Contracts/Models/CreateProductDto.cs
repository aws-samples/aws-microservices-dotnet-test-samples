using System.ComponentModel.DataAnnotations;

namespace InventoryService.Contracts.Models;

public record CreateProductDto(
    [Required] 
    string Name,
    
    [RegularExpression(@"^\$?\d+(\.(\d{2}))?$")]
    double Price, 
    
    [Required] 
    uint Quantity = 0);