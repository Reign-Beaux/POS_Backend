using Application.Features.Brands.UseCases.Commands.Create;
using AutoMapper;
using Domain.Entities.Brands;

namespace Application.Features.Brands
{
    public class BrandMappings : Profile
    {
        public BrandMappings()
        {
            CreateMap<BrandCreateCommand, Brand>();
        }
    }
}
