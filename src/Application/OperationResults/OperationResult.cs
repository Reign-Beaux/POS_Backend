using Application.Behaviors;
using MediatR;
using System.Net;

namespace Application.OperationResults
{
    public class OperationResult<TValue>(HttpStatusCode statusCode, TValue? value = default, ErrorDetails? errorDetails = null)
    {
        public TValue? Value { get; } = value;
        public HttpStatusCode Status { get; set; } = statusCode;
        public ErrorDetails? ErrorDetails { get; } = errorDetails;

        public static implicit operator OperationResult<TValue>(OperationResult<Unit> unitResult)
        {
            return new OperationResult<TValue>(unitResult.Status, default, unitResult.ErrorDetails);
        }
    }

    public class OperationResult
    {
        public static OperationResult<TValue> Success<TValue>(TValue value)
            => new(HttpStatusCode.OK, value);

        public static OperationResult<Unit> Success()
            => new(HttpStatusCode.OK);

        public static OperationResult<TValue> CreatedAtAction<TValue>(TValue value)
            => new(HttpStatusCode.Created, value: value);

        public static OperationResult<Unit> NoContent()
            => new(HttpStatusCode.NoContent);

        public static OperationResult<Unit> Validations(List<ValidationError> errors)
        {
            var errorDetails = new ErrorDetails
            {
                Title = ValidationMessages.Title,
                Message = ValidationMessages.Message,
                Errors = errors
            };

            return new(HttpStatusCode.BadRequest, errorDetails: errorDetails);
        }

        public static OperationResult<Unit> Conflict(string message)
            => CreateResultWithErrors(HttpStatusCode.Conflict, title: nameof(HttpStatusCode.Conflict), message);

        public static OperationResult<Unit> BadRequest(string message)
            => CreateResultWithErrors(HttpStatusCode.BadRequest, title: nameof(HttpStatusCode.BadRequest), message);

        public static OperationResult<Unit> NotFound(string message)
            => CreateResultWithErrors(HttpStatusCode.NotFound, title: nameof(HttpStatusCode.NotFound), message);

        public static OperationResult<Unit> InternalServerError(string message)
            => CreateResultWithErrors(HttpStatusCode.InternalServerError, title: nameof(HttpStatusCode.InternalServerError), message);

        public static OperationResult<Unit> Unauthorized(string message)
            => CreateResultWithErrors(HttpStatusCode.Unauthorized, title: nameof(HttpStatusCode.Unauthorized), message);

        private static OperationResult<Unit> CreateResultWithErrors(HttpStatusCode statucCode, string title, string message)
            => new(
                statucCode,
                errorDetails: new()
                {
                    Title = title,
                    Message = message
                });
    }
}
