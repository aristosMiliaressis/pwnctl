using pwnctl.app.Common.Exceptions;

namespace pwnctl.infra.Configuration.Validation.Exceptions;

public sealed class ConfigValidationException : AppException
{
    public ConfigValidationException(string fileName, string? errorMessage)
        : base($"Configuration file {fileName} failed with error: {errorMessage}")
    { }

    public ConfigValidationException(string fileName, string errorMessage, Exception innerEx)
        : base($"Configuration file {fileName} failed with error: {errorMessage}", innerEx)
    { }
}