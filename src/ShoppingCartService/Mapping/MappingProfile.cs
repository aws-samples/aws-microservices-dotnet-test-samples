using AutoMapper;
using ShoppingCartService.BusinessLogic.Models;
using ShoppingCartService.Controllers.Models;
using ShoppingCartService.DataAccess.Entities;

namespace ShoppingCartService.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ShippingAddressDto, ShippingAddress>().ReverseMap();
        CreateMap<ShoppingCartDo, ShoppingCartDto>();
    }
}