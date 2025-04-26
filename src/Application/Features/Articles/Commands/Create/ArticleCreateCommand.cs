using Application.OperationResults;
using MediatR;

namespace Application.Features.Articles.Commands.Create
{
    public record ArticleCreateCommand(
        Guid ArticleTypeId,
        Guid BrandId,
        string Name,
        string Description,
        decimal Stock,
        decimal MinStockLevel,
        decimal MaxStockLevel,
        string BarCode) : IRequest<OperationResult<Unit>>;
}
