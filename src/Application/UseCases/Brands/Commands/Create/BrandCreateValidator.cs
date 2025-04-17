using FluentValidation;

namespace Application.UseCases.Brands.Commands.Create
{
    public class BrandCreateValidator : AbstractValidator<BrandCreateCommand>
    {
        public BrandCreateValidator()
        {
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
        }
    }
}
