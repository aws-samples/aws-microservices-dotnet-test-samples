using InventoryService.BusinessLogic;
using InventoryService.Contracts.Models;
using InventoryService.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly InventoryManager _inventoryManager;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(InventoryManager inventoryManager, ILogger<InventoryController> logger)
    {
        _inventoryManager = inventoryManager;
        _logger = logger;
    }

    /// <summary>
    ///     Get all products in inventory
    /// </summary>
    [HttpGet]
    public IEnumerable<ProductDto> GetAll()
    {
        return _inventoryManager.GetAll();
    }

    /// <summary>
    ///     Get product by id
    /// </summary>
    /// <param name="id">Product id</param>
    [HttpGet("{id}", Name = "GetProduct")]
    public ActionResult<ProductDto> FindById(string id)
    {
        ProductDto findById;
        try
        {
            findById = _inventoryManager.FindById(id);
        }
        catch (ProductNotFoundException exception)
        {
            _logger.LogDebug("Product {Id}, not found - {ExMessage}", id, exception.Message);
            return NotFound();
        }

        return findById;
    }

    /// <summary>
    ///     Add a new product to the inventory
    /// </summary>
    [HttpPost]
    public ActionResult<ProductDto> AddToInventory([FromBody] CreateProductDto createProduct)
    {
        var result = _inventoryManager.AddToInventory(createProduct);

        return CreatedAtRoute("GetProduct", new { id = result.Id }, result);
    }

    [HttpPut("{id}/quantity")]
    public ActionResult<ProductDto> UpdateQuantity(string id, [FromBody] uint quantity)
    {
        try
        {
            var result = _inventoryManager.UpdateProductQuantity(id, quantity);

            return result;
        }
        catch (ProductNotFoundException ex)
        {
            _logger.LogWarning("Cannot update quantity - {ExMessage}", ex.Message);

            return NotFound();
        }
    }
}