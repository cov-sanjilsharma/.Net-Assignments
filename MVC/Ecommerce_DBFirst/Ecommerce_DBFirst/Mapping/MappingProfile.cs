using AutoMapper;
using Ecommerce_DBFirst.Dtos;
using Ecommerce_DBFirst.Models;
namespace Ecommerce_DBFirst.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Products, ProductDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
