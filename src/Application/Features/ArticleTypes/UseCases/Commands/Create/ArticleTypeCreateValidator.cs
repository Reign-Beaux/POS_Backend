using Application.Shared.Catalogs;
using Domain.Entities;
using FluentValidation;

namespace Application.Features.ArticleTypes.UseCases.Commands.Create
{
    public class ArticleTypeCreateValidator : AbstractValidator<ArticleTypeCreateCommand>
    {
        public ArticleTypeCreateValidator()
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
