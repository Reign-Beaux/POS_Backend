using Application.DTOs.Brands;
using AutoMapper;
using Domain.Entities.Brands;

namespace Application.Mappings
{
    public class BrandMappings : Profile
    {
        public BrandMappings()
        {
            CreateMap<Brand, BrandDTO>();
        }
    }
}
