using Microsoft.AspNetCore.Mvc;
using ShoppingCartService.BusinessLogic;
using ShoppingCartService.Controllers.Models;
using ShoppingCartService.Exceptions;

namespace ShoppingCartService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShoppingCartController : ControllerBase
{
    private readonly ILogger<ShoppingCartController> _logger;
    private readonly ShoppingCartManager _shoppingCartManager;

    public ShoppingCartController(ShoppingCartManager shoppingCartManager, ILogger<ShoppingCartController> logger)
    {
        _shoppingCartManager = shoppingCartManager;
        _logger = logger;
    }

   

    /// <summary>
    ///     Get cart by id
    /// </summary>
    /// <param name="id">Shopping cart id</param>
    [HttpGet("{id}", Name = "GetCart")]
    public async Task<ActionResult<ShoppingCartDto>> FindById(string id)
    {
        try
        {
            return  await _shoppingCartManager.FindById(id);
        }
        catch (ShoppingCartNotFoundException exception)
        {
            _logger.LogWarning("Cannot find shopping cart {Id} : {Message}", id, exception.Message);
            return NotFound();
        }
    }
    
    /// <summary>
    ///     Create a new shopping cart
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ShoppingCartDto>> Create([FromBody] CreateShoppingCartDto createCart)
    {
        var result = await _shoppingCartManager.CreateAsync(createCart);

        return CreatedAtRoute("GetCart", new {id = result.Id}, result);
    }
    
    /// <summary>
    ///     Add product to existing cart
    /// </summary>
    /// <param name="id">Shopping cart id</param>
    /// <param name="productId">The item's product id</param>
    [HttpPut("{id}/item/{productId}")]
    public async Task<ActionResult<ShoppingCartDto>> AddItemToCart(string id, string productId)
    {
        try
        {
            var shoppingCartDto = await _shoppingCartManager.AddItemToCart(id, productId);
            return  Ok(shoppingCartDto);
        }
        catch (ShoppingCartNotFoundException exception)
        {
            _logger.LogWarning("Cannot find shopping cart {Id} : {Message}", id, exception.Message);

            return BadRequest();
        }
        catch (ProductNotFoundException exception)
        {
            _logger.LogWarning("Cannot add product {Id} to shopping cart: {Message}", id, exception.Message);

            return NotFound();
        }
    }

    /// <summary>
    /// Finlize shopping cart and send to order processing
    /// </summary>
    /// <param name="id">Shopping cart id</param>
    [HttpPost("/checkout/{id}")]
    public async Task<IActionResult> Checkout(string id)
    {
        try
        {
            await _shoppingCartManager.CheckoutAsync(id);

            return Ok();
        }
        catch (ShoppingCartNotFoundException exception)
        {
            _logger.LogWarning("Failed checkout - shopping cart {Id} not found: {Message}", id, exception.Message);

            return NotFound();
        }
        catch (NoItemsInShoppingCartException exception)
        {
            _logger.LogWarning("Failed checkout - shopping cart {Id} does not have any items: {Message}", id,
                exception.Message);

            return BadRequest();
        }
        catch (ApplicationException exception)
        {
            _logger.LogWarning("Failed checkout - failed creating order: {Message}", exception.Message);

            return BadRequest();
        }
    }
}