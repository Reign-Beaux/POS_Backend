using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class BaseMappings : Profile
    {
        public BaseMappings()
        {
            CreateMap<BaseCatalogs, CatalogDTO>();
        }
    }
}
