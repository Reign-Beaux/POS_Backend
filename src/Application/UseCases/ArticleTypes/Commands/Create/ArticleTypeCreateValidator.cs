using FluentValidation;

namespace Application.UseCases.ArticleTypes.Commands.Create
{
    public class ArticleTypeCreateValidator : AbstractValidator<ArticleTypeCreateCommand>
    {
        public ArticleTypeCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(100)
                .WithMessage("Name must not exceed 100 characters.");
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .MaximumLength(256)
                .WithMessage("Description must not exceed 256 characters.");
        }
    }
}
