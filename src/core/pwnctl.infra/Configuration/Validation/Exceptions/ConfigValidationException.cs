using pwnctl.app.Common.Exceptions;

namespace pwnctl.infra.Configuration.Validation.Exceptions;

public sealed class ConfigValidationException : AppException
{
    public ConfigValidationException(string fileName, string? errorMessage)
        : base($"Configuration file {fileName} failed with error: {errorMessage}")
    { }
}