using Application.Shared.Catalogs;
using Domain.Entities;
using FluentValidation;

namespace Application.Features.Brands.UseCases.Commands.Create
{
    public class BrandCreateValidator : AbstractValidator<BrandCreateCommand>
    {
        public BrandCreateValidator()
        {
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
