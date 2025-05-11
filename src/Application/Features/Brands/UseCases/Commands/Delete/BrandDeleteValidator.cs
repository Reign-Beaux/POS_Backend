using FluentValidation;

namespace Application.Features.Brands.UseCases.Commands.Delete
{
    public class BrandDeleteValidator : AbstractValidator<BrandDeleteCommand>
    {
        public BrandDeleteValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(BrandCachedKeys.IdIsRequired);
        }
    }
}
