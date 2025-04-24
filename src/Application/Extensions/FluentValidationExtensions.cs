using FluentValidation;

namespace Application.Extensions
{
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> WithMessageAsync<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> ruleBuilderOptions,
            Task<string> messageTask)
        {
            return (IRuleBuilderOptions<T, TProperty>)ruleBuilderOptions.CustomAsync(async (value, context, cancellation) =>
            {
                var message = await messageTask;
                
                if (!string.IsNullOrWhiteSpace(message))
                {
                    context.AddFailure(message);
                }
                else
                {
                    context.AddFailure("Validation failed, but no message was provided.");
                }
            });
        }
    }
}
