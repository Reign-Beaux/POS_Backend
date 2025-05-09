using Application.Features.Brands.DTOs;
using Application.Features.Brands.UseCases.Commands.Create;
using Application.Features.Brands.UseCases.Commands.Update;
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
            CreateMap<BrandUpdateCommand, Brand>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
