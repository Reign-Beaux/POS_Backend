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
                .WithMessageAsync(localization.GetText("Catalog_NameIsRequired"))
                .MaximumLength(BaseCatalog.MaxNameLength)
                .WithMessageAsync(localization.GetText("Catalog_NameMaxLength"));
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessageAsync(localization.GetText("Catalog_DescriptionIsRequired"))
                .MaximumLength(BaseCatalog.MaxDescriptionLength)
                .WithMessageAsync(localization.GetText("Catalog_DescriptionMaxLength"));
        }
    }
}
