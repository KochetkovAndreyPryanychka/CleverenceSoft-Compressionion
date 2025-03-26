namespace Compression.Exceptions;

public class IncorrectCompressedDataException : Exception
{
    public IncorrectCompressedDataException(string message) : base(message) {}
}