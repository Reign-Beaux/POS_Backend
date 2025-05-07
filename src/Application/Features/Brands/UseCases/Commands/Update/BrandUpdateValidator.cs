using Application.Shared.Catalogs;
using Domain.Entities;
using FluentValidation;

namespace Application.Features.Brands.UseCases.Commands.Update
{
    public class BrandUpdateValidator : AbstractValidator<BrandUpdateCommand>
    {
        public BrandUpdateValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Brand ID is required.");
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(CatalogCachedKeys.NameIsRequired)
                .MaximumLength(BaseCatalog.NameMaxLength)
                .WithMessage(CatalogCachedKeys.NameMaxLength);
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage(CatalogCachedKeys.DescriptionIsRequired)
                .MaximumLength(BaseCatalog.DescriptionMaxLength)
                .WithMessage(CatalogCachedKeys.DescriptionMaxLength);
        }
    }
}
