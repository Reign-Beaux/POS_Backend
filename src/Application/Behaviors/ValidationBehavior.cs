using Application.Interfaces.Caching;
using Application.OperationResults;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>(
        ILocalizationCached localization,
        IValidator<TRequest>? validator = null) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IValidator<TRequest>? _validator = validator;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (_validator is null)
            {
                return await next(cancellationToken);
            }

            var validatorResult = await _validator.ValidateAsync(request, cancellationToken);

            if (validatorResult.IsValid)
            {
                return await next(cancellationToken);
            }

            List<ValidationError> validationErrors = [];

            foreach (ValidationFailure error in validatorResult.Errors)
            {
                string errorMessage = await localization.GetText(error.ErrorMessage);
                ValidationError validationError = new()
                {
                    PropertyName = error.PropertyName,
                    ErrorMessage = errorMessage
                };
                validationErrors.Add(validationError);
            };

            return (dynamic)OperationResult.Validations(validationErrors);
        }
    }
}
