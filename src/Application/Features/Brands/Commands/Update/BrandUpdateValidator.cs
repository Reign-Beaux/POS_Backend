using FluentValidation;

namespace Application.Features.Brands.Commands.Update
{
    public class BrandUpdateValidator : AbstractValidator<BrandUpdateCommand>
    {
        public BrandUpdateValidator()
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
