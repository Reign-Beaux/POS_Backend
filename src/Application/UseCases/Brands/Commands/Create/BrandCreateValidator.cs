using Application.Languages;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Brands.Commands.Create
{
    public class BrandCreateValidator : AbstractValidator<BrandCreateCommand>
    {
        public BrandCreateValidator(Language language)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(language.GetString(ResourcesTypes.CatalogMessages, "NameIsRequired"))
                .MaximumLength(BaseCatalog.MaxNameLength)
                .WithMessage(language.GetString(ResourcesTypes.CatalogMessages, "NameMaxLength"));
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage(language.GetString(ResourcesTypes.CatalogMessages, "DescriptionIsRequired"))
                .MaximumLength(BaseCatalog.MaxDescriptionLength)
                .WithMessage(language.GetString(ResourcesTypes.CatalogMessages, "DescriptionMaxLength"));
        }
    }
}
