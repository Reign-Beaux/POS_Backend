using Application.DTOs.ArticleTypes;
using AutoMapper;
using Domain.Entities.ArticleTypes;

namespace Application.Mappings
{
    public class ArticleTypeMappings : Profile
    {
        public ArticleTypeMappings()
        {
            CreateMap<ArticleType, ArticleTypeDTO>();
        }
    }
}
