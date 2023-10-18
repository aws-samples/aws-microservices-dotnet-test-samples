
using AutoMapper;
using ShoppingCartService.BusinessLogic.Models;
using ShoppingCartService.Controllers.Models;
using ShoppingCartService.DataAccess;
using ShoppingCartService.Exceptions;

namespace ShoppingCartService.BusinessLogic;

public class ShoppingCartManager
{
    private readonly IMapper _mapper;
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IOrderServiceNotifications _orderServiceNotifications;

    public ShoppingCartManager(
        IShoppingCartRepository shoppingCartRepository, 
        IInventoryRepository inventoryRepository,
        IOrderServiceNotifications orderServiceNotifications,
        IMapper mapper)
    {
        _mapper = mapper;
        _orderServiceNotifications = orderServiceNotifications;
        _shoppingCartRepository = shoppingCartRepository;
        _inventoryRepository = inventoryRepository;
    }
    public async Task<ShoppingCartDto> CreateAsync(CreateShoppingCartDto createCart)
    {
        var shippingAddress = _mapper.Map<ShippingAddress>(createCart.ShippingAddress);

        var shoppingCartDo = await _shoppingCartRepository.CreateAsync(shippingAddress);

        return _mapper.Map<ShoppingCartDto>(shoppingCartDo);
    }

    public async Task<ShoppingCartDto> FindById(string shoppingCartId)
    {
        var shoppingCartDo = await _shoppingCartRepository.FindByIdAsync(shoppingCartId);
        if (shoppingCartDo is null)
        {
            throw new ShoppingCartNotFoundException("Id not found");
        }

        return _mapper.Map<ShoppingCartDto>(shoppingCartDo);
    }

    public async Task<ShoppingCartDto> AddItemToCart(string shoppingCartId, string productId)
    {
        var productExist = await _inventoryRepository.ProductExist(productId);
        if (productExist is false)
        {
            throw new ProductNotFoundException("Product not found");
        }

        var shoppingCartDo = await _shoppingCartRepository.AddItemToCart(shoppingCartId, productId);
        if (shoppingCartDo is null)
        {
            throw new ShoppingCartNotFoundException("Shopping cart not found");
        }

        return _mapper.Map<ShoppingCartDto>(shoppingCartDo);
    }

    public async Task CheckoutAsync(string shoppingCartId)
    {
        var shoppingCartDo = await _shoppingCartRepository.FindByIdAsync(shoppingCartId);
        if (shoppingCartDo is null)
        {
            throw new ShoppingCartNotFoundException("Shopping cart not found");
        }

        if (shoppingCartDo.Items.Any() is false)
        {
            throw new NoItemsInShoppingCartException(
                $"Cannot send order, shopping cart {shoppingCartId} does not have any items");
        }
        
        await _orderServiceNotifications.SendOrder(shoppingCartDo.Items, shoppingCartDo.ShippingAddress);
    }
}