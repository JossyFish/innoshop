using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProductService.Domain.Exceptions;


namespace ProductService.API.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            var problemDetails = exception switch
            {
                InvalidCurrencyException invalidCurrency => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Invalid currency error",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = exception.Message,
                    Extensions = { ["currency"] = ((InvalidCurrencyException)exception).Currency}
                },

                InvalidOperation invalidOperation => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Invalid operation",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = exception.Message,
                    Extensions = { ["operation"] = invalidOperation.Operation }
                },

                ProductUserNotFoundException userNotFound => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = exception.Message,
                    Extensions = { ["userId"] = userNotFound.Id }
                },

                ProductNotFoundException productNotFound => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = exception.Message,
                    Extensions = { ["productId"] = productNotFound.Id }
                },

                ProductAccessDeniedException productAccessDenied => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                    Title = "Access Denied",
                    Status = StatusCodes.Status403Forbidden,
                    Detail = exception.Message,
                    Extensions = { ["productId"] = productAccessDenied.ProductId, ["userId"] = productAccessDenied.UserId}
                },

                Domain.Exceptions.ValidationException validation => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Validation error",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = exception.Message,
                    Extensions = { ["errors"] = ((Domain.Exceptions.ValidationException)exception).Errors }
                },

                _ => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Title = "Server error",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "Server error has occurred"
                }
            };

            context.Response.StatusCode = problemDetails.Status ?? 500;
            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}
