
namespace UserService.Domain.Exceptions
{
    public class ValidationException : Exception
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(Dictionary<string, string[]> errors)
            : base("Validation errors occurred")
        {
            Errors = errors;
        }
    }
}
