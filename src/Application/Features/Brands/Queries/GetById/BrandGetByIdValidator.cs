using FluentValidation;

namespace Application.Features.Brands.Queries.GetById
{
    public class BrandGetByIdValidator : AbstractValidator<BrandGetByIdQuery>
    {
        public BrandGetByIdValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Brand ID cannot be empty.");
        }
    }
}
