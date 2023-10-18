using AutoMapper;
using InventoryService.Contracts.Models;
using InventoryService.DataAccess;
using InventoryService.DataAccess.Entities;
using InventoryService.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.BusinessLogic;

public class InventoryManager
{
    private readonly IMapper _mapper;
    private readonly IProductRepository _productRepository;

    public InventoryManager(IMapper mapper, IProductRepository productRepository)
    {
        _mapper = mapper;
        _productRepository = productRepository;
    }
    
    public ProductDto AddToInventory(CreateProductDto createProduct)
    {
        var product = _mapper.Map<Product>(createProduct);

        var result = _productRepository.Create(product);

        return _mapper.Map<ProductDto>(result);
    }
    
    public ProductDto FindById(string id)
    {
        var result = _productRepository.FindById(id);

        if (result == null)
        {
            throw new ProductNotFoundException($"product {id} not found in inventory");
        }

        return _mapper.Map<ProductDto>(result);
    }
    
    public IEnumerable<ProductDto> GetAll()
    {
        var result = _productRepository.FindAll();

        return _mapper.Map<IEnumerable<ProductDto>>(result);
    }

    public ActionResult<ProductDto> UpdateProductQuantity(string id, uint quantity)
    {
        var product = _productRepository.FindById(id);
        if (product == null)
        {
            throw new ProductNotFoundException($"product {id} not found in inventory" );
        }

        product.Quantity = quantity;

        _productRepository.Update(product.Id, product);

        return _mapper.Map<ProductDto>(product);
    }
}
