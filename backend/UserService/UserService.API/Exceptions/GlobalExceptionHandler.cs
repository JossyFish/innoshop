using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserService.Domain.Exceptions;

namespace UserService.API.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            var problemDetails = exception switch
            {
                UserAlreadyExistException userExists => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                    Title = "Conflict",
                    Status = StatusCodes.Status409Conflict,
                    Detail = exception.Message,
                    Extensions = { ["email"] = ((UserAlreadyExistException)exception).Email }
                },

                UserNotFoundException userNotFound => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = exception.Message,
                    Extensions = { ["email"] = userNotFound.Email }
                },

                UserNotFoundByIdException userNotFound => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = exception.Message,
                    Extensions = { ["userId"] = userNotFound.Id }
                },

                UserUnauthorizedException unauthorized => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                    Title = "Unauthorized",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = exception.Message
                },

                ConfirmCodeExpiredException codeExpired => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                    Title = "Code Expired",
                    Status = StatusCodes.Status400BadRequest, 
                    Detail = exception.Message,
                    Extensions = { ["confirmCode"] = codeExpired.ConfirmCode }
                },

                InvalidConfirmCodeException invalidCode => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Invalid Code",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = exception.Message,
                    Extensions = { ["confirmCode"] = invalidCode.ConfirmCode }
                },

                InvalidLinkTokenException invalidLinkToken => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Invalid Link Token",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = exception.Message,
                    Extensions = { ["linkToken"] = invalidLinkToken.Message }
                },

                CacheDataNotFoundException cacheNotFound => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "Data Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = exception.Message,
                    Extensions = {
                    ["dataType"] = cacheNotFound.DataType,
                    ["identifier"] = cacheNotFound.Identifier
                }
                },

                InvalidPasswordException invalidPassword => new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Invalid Password",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = exception.Message,
                    Extensions = { ["password"] = invalidPassword.Password }
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
