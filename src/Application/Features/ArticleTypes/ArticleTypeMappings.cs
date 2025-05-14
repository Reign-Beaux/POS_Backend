using Domain.Entities.ArticleTypes;
using AutoMapper;
using Application.Features.ArticleTypes.DTOs;
using Application.Features.ArticleTypes.UseCases.Commands.Create;

namespace Application.Features.ArticleTypes
{
    public class ArticleTypeMappings : Profile
    {
        public ArticleTypeMappings()
        {
            CreateMap<ArticleType, ArticleTypeDTO>();
            CreateMap<ArticleTypeCreateCommand, ArticleType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<ArticleTypeUpdateCommand, ArticleType>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
