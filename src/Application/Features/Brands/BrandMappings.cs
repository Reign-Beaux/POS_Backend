using Application.Features.Brands.DTOs;
using Application.Features.Brands.UseCases.Commands.Create;
using AutoMapper;
using Domain.Entities.Brands;

namespace Application.Features.Brands
{
    public class BrandMappings : Profile
    {
        public BrandMappings()
        {
            CreateMap<Brand, BrandDTO>();
            CreateMap<BrandCreateCommand, Brand>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
