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

        public static OperationResult<Unit> Validations(List<ValidationError> errors)
        {
            var errorDetails = new ErrorDetails
            {
                Title = "Validation Failed",
                Message = "Error en validaciones",
                Errors = errors
            };

            return new(HttpStatusCode.BadRequest, errorDetails: errorDetails);
        }

        public static OperationResult<Unit> Conflict(string message)
            => CreateResultWithErrors(HttpStatusCode.Conflict, title: "Conflict", message);

        public static OperationResult<Unit> BadRequest(string message)
            => CreateResultWithErrors(HttpStatusCode.BadRequest, title: "Bad Request", message);

        public static OperationResult<Unit> NotFound(string message)
            => CreateResultWithErrors(HttpStatusCode.NotFound, title: "Not Found", message);

        public static OperationResult<Unit> InternalServerError(string message)
            => CreateResultWithErrors(HttpStatusCode.InternalServerError, title: "Internal Server Error", message);

        public static OperationResult<Unit> Unauthorized(string message)
            => CreateResultWithErrors(HttpStatusCode.Unauthorized, title: "Unauthorized", message);

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
