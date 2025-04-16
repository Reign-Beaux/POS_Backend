using Application.DTOs.ArticleTypes;
using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using AutoMapper;
using Domain.Entities.ArticleTypes;
using MediatR;

namespace Application.UseCases.ArticleTypes.Queries.GetAll
{
    public sealed class ArticleTypeGetAllHandler(
        IMapper mapper,
        IPosDbUnitOfWork posDb) : IRequestHandler<ArticleTypeGetAllQuery, OperationResult<IEnumerable<ArticleTypeDTO>>>
    {
        public async Task<OperationResult<IEnumerable<ArticleTypeDTO>>> Handle(ArticleTypeGetAllQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<ArticleType> articleTypes = await posDb.ArticleTypeRepository.GetAll();
                IEnumerable<ArticleTypeDTO> articleTypeDTOs = mapper.Map<IEnumerable<ArticleTypeDTO>>(articleTypes);
                return OperationResult.Success(articleTypeDTOs);
            }
            catch (Exception ex)
            {
                return OperationResult.InternalServerError(ex.Message);
            }
        }
    }
}
