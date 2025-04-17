using Application.Interfaces.UnitOfWorks;
using Application.OperationResults;
using Domain.Entities.Brands;
using MediatR;

namespace Application.UseCases.Brands.Commands.Create
{
    public sealed class BrandCreateHandler(IPosDbUnitOfWork posDb) : IRequestHandler<BrandCreateCommand, OperationResult<Unit>>
    {
        public async Task<OperationResult<Unit>> Handle(BrandCreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Brand brand = new()
                {
                    Name = request.Name,
                    Description = request.Description
                };
                posDb.BrandRepository.Add(brand, cancellationToken);
                await posDb.SaveChangesAsync(cancellationToken);

                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                return OperationResult.InternalServerError(ex.Message);
            }
        }
    }
}
