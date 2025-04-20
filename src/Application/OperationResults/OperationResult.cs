using MediatR;
using System.Net;

namespace Application.OperationResults
{
    public class OperationResult<TValue>(bool isSuccess = true, TValue? value = default, ErrorDetails? errorDetails = null)
    {
        public bool IsSuccess { get; } = isSuccess;
        public TValue? Value { get; } = value;
        public ErrorDetails? ErrorDetails { get; } = errorDetails;

        public static implicit operator OperationResult<TValue>(OperationResult<Unit> unitResult)
        {
            return new OperationResult<TValue>(unitResult.IsSuccess, default, unitResult.ErrorDetails);
        }
    }

    public class OperationResult
    { 
        public static OperationResult<TValue> Success<TValue>(TValue value)
            => new(value: value);

        public static OperationResult<Unit> Success()
            => new();

        public static OperationResult<Unit> Failure(ErrorDetails errorDetails)
            => new(isSuccess: false, errorDetails: errorDetails);

        public static OperationResult<Unit> Validations(List<ValidationError> errors)
        {
            var errorDetails = new ErrorDetails
            {
                Status = HttpStatusCode.BadRequest,
                Title = "Validation Failed",
                Message = "Error en validaciones",
                Errors = errors
            };

            return Failure(errorDetails);
        }

        public static OperationResult<Unit> BadRequest(string message)
            => Failure(CreateErrorDetails(HttpStatusCode.BadRequest, title: "Bad Request", message));

        public static OperationResult<Unit> NotFound(string message)
            => Failure(CreateErrorDetails(HttpStatusCode.NotFound, title: "Not Found", message));

        public static OperationResult<Unit> InternalServerError(string message)
            => Failure(CreateErrorDetails(HttpStatusCode.InternalServerError, title: "Internal Server Error", message));

        public static OperationResult<Unit> Unauthorized(string message)
            => Failure(CreateErrorDetails(HttpStatusCode.Unauthorized, title: "Unauthorized", message));

        private static ErrorDetails CreateErrorDetails(HttpStatusCode status, string title, string message)
        {
            return new()
            {
                Status = status,
                Title = title,
                Message = message
            };
        }
    }
}
