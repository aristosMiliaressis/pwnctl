namespace pwnctl.app.Tasks.Validation.Exceptions;

public sealed class ConfigValidationException : Exception
{
    public ConfigValidationException(string fileName, string errorMessage)
        : base($"Configuration file {fileName} failed with error: {errorMessage}")
    { }
}