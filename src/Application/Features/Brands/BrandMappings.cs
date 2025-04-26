using Application.Features.Brands.DTOs;
using AutoMapper;
using Domain.Entities.Brands;

namespace Application.Features.Brands
{
    public class BrandMappings : Profile
    {
        public BrandMappings()
        {
            CreateMap<Brand, BrandDTO>();
        }
    }
}
