using System.Net;

namespace Application.OperationResults
{
    public class ErrorDetails
    {
        public string Title { get; set; } = string.Empty;
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<ValidationError>? Errors { get; set; } = null;
    }
}
