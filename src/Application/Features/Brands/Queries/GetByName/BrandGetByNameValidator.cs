using FluentValidation;

namespace Application.Features.Brands.Queries.GetByName
{
    public class BrandGetByNameValidator : AbstractValidator<BrandGetByNameQuery>
    {
        public BrandGetByNameValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Brand name cannot be empty.")
                .MaximumLength(64)
                .WithMessage("Brand name cannot exceed 64 characters.");
        }
    }
}
