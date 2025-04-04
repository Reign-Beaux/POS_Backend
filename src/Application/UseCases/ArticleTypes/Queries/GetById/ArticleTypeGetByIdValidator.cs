using FluentValidation;

namespace Application.UseCases.ArticleTypes.Queries.GetById
{
    public class ArticleTypeGetByIdValidator : AbstractValidator<ArticleTypeGetByIdQuery>
    {
        public ArticleTypeGetByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Article type ID cannot be empty.");
        }
    }
    {
    }
}
