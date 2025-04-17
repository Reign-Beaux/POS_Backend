using Application.DTOs.Articles;
using Application.DTOs;
using Application.UseCases.Articles.Commands.Create;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Articles;

namespace Application.Mappings
{
    public class ArticleMappings : Profile
    {
        public ArticleMappings()
        {
            CreateMap<ArticleCreateCommand, Article>();

            CreateMap<Article, ArticleDTO>()
                .ForMember(dest => dest.ArticleTypeName, opt => opt.MapFrom(src => src.ArticleType!.Name))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand!.Name))
                .IncludeBase<BaseCatalogs, CatalogDTO>();
        }
    }
}
