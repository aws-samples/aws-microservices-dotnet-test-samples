using AutoMapper;
using InventoryService.Contracts.Models;
using InventoryService.DataAccess.Entities;

namespace InventoryService.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateProductDto, Product>();
        CreateMap<Product, ProductDto>();
        CreateMap<Product?, ProductDto?>();
    }
}