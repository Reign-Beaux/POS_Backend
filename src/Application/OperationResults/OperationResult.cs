using MediatR;
using System.Net;

namespace Application.OperationResults
{
    public class OperationResult<TValue>(bool isSuccess = true, TValue? value = default, ErrorDetails? problemDetails = null)
    {
        public bool IsSuccess { get; } = isSuccess;
        public TValue? Value { get; } = value;
        public ErrorDetails? ProblemDetails { get; } = problemDetails;

        public static implicit operator OperationResult<TValue>(OperationResult<Unit> unitResult)
        {
            return new OperationResult<TValue>(unitResult.IsSuccess, default, unitResult.ProblemDetails);
        }
    }

    public class OperationResult
    { 
        public static OperationResult<TValue> Success<TValue>(TValue value)
            => new(value: value);

        public static OperationResult<Unit> Success()
            => new();

        public static OperationResult<Unit> Failure(ErrorDetails problemDetails)
            => new(isSuccess: false, problemDetails: problemDetails);

        public static OperationResult<Unit> Validations(List<ValidationError> errors)
        {
            var problemDetails = new ErrorDetails
            {
                Status = HttpStatusCode.BadRequest,
                Title = "Validation Failed",
                Message = "Error en validaciones",
                Errors = errors
            };

            return Failure(problemDetails);
        }

        public static OperationResult<Unit> BadRequest(string message)
            => Failure(CreateProblemDetails(HttpStatusCode.BadRequest, title: "Bad Request", message));

        public static OperationResult<Unit> NotFound(string message)
            => Failure(CreateProblemDetails(HttpStatusCode.NotFound, title: "Not Found", message));

        public static OperationResult<Unit> InternalServerError(string message)
            => Failure(CreateProblemDetails(HttpStatusCode.InternalServerError, title: "Internal Server Error", message));

        public static OperationResult<Unit> Unauthorized(string message)
            => Failure(CreateProblemDetails(HttpStatusCode.Unauthorized, title: "Unauthorized", message));

        private static ErrorDetails CreateProblemDetails(HttpStatusCode status, string title, string message)
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
