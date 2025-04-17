using FluentValidation;

namespace Application.UseCases.Articles.Commands.Create
{
    public class ArticleCreateValidator : AbstractValidator<ArticleCreateCommand>
    {
        public ArticleCreateValidator()
        {
            RuleFor(x => x.ArticleTypeId)
                .NotEmpty()
                .WithMessage("Article type is required.");
            RuleFor(x => x.BrandId)
                .NotEmpty()
                .WithMessage("Brand is required.");
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(64)
                .WithMessage("Name must not exceed 64 characters.");
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .MaximumLength(256)
                .WithMessage("Description must not exceed 256 characters.");
            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Stock must be greater than or equal to 0.");
            RuleFor(x => x.MinStockLevel)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Min stock level must be greater than or equal to 0.");
            RuleFor(x => x.MaxStockLevel)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Max stock level must be greater than or equal to 0.");
            RuleFor(x => x.BarCode)
                .NotEmpty()
                .WithMessage("Bar code is required.")
                .MaximumLength(64)
                .WithMessage("Bar code must not exceed 64 characters.");
        }
    }
}
