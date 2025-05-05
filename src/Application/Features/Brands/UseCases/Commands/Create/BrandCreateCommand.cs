using Application.OperationResults;
using Application.Shared.Catalogs;
using MediatR;

namespace Application.Features.Brands.UseCases.Commands.Create
{
    public record BrandCreateCommand : CatalogDTOAbstraction, IRequest<OperationResult<Guid>>;
}
