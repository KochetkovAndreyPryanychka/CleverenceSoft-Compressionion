namespace Compression;

public static class CharConsistency
{
    public static bool IsCharNumber(char symbol) =>
        symbol is >= '0' and <= '9';

    public static bool IsCharLowerCaseLetter(char symbol) =>
        symbol is >= 'a' and <= 'z';
}