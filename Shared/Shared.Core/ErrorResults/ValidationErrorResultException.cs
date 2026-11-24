namespace snowcoreBlog.Backend.Core;

public sealed class ValidationErrorResultException : Exception
{
    public ValidationErrorResultException() : base($"{nameof(ValidationErrorResult)} cannot be created from a successful validation") { }
}
