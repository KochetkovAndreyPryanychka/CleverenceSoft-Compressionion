namespace Compression.Exceptions;

public class InvalidAppSettingsException : Exception
{
    public InvalidAppSettingsException(string message) : base(message) {}
}