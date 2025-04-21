using Application.Languages;
using Domain.Entities;
using FluentValidation;

namespace Application.UseCases.Brands.Commands.Create
{
    public class BrandCreateValidator : AbstractValidator<BrandCreateCommand>
    {
        public BrandCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(Language.GetString("NameIsRequired"))
                .MaximumLength(BaseCatalog.MaxNameLength)
                .WithMessage(Language.GetString("NameMaxLength"));
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage(Language.GetString("DescriptionIsRequired"))
                .MaximumLength(BaseCatalog.MaxDescriptionLength)
                .WithMessage(Language.GetString("DescriptionMaxLength"));
        }
    }
}
