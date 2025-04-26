using Application.Constants.CachedKeys;
using Application.Extensions;
using Application.Interfaces.Caching;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Brands.Commands.Create
{
    public class BrandCreateValidator : AbstractValidator<BrandCreateCommand>
    {
        public BrandCreateValidator(ILocalizationCached localization)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessageAsync(localization.GetText(CatalogCachedKeys.NameIsRequired))
                .MaximumLength(BaseCatalog.NameMaxLength)
                .WithMessageAsync(localization.GetText(CatalogCachedKeys.NameMaxLength));
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessageAsync(localization.GetText(CatalogCachedKeys.DescriptionIsRequired))
                .MaximumLength(BaseCatalog.DescriptionMaxLength)
                .WithMessageAsync(localization.GetText(CatalogCachedKeys.DescriptionMaxLength));
        }
    }
}
