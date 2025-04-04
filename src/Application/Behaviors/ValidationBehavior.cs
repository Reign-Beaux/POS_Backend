using Application.OperationResults;
using FluentValidation;
using MediatR;

namespace Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null) : IPipelineBehavior<TRequest, TResponse>
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
                return await next();
            }

            var validatorResult = await _validator.ValidateAsync(request, cancellationToken);

            if (validatorResult.IsValid)
            {
                return await next();
            }

            var validationErrors = validatorResult.Errors
                .Select(validationFailure => new ValidationError
                {
                    PropertyName = validationFailure.PropertyName,
                    ErrorMessage = validationFailure.ErrorMessage
                })
                .ToList();

            return (dynamic)OperationResult.Validations(validationErrors);
        }
    }
}
