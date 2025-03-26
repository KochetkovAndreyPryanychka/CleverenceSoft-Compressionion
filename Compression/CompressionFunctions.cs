using System.Text;
using Compression.Exceptions;

namespace Compression;

public static class CompressionFunctions
{
    public static StringBuilder CompressString(string inputString)
    {
        var sb = new StringBuilder();
        
        char? currentChar = null;
        int? currentBatch = null;
        foreach (var c in inputString)
        {
            if (c != currentChar)
            {
                sb.Append(currentChar);
                if (currentBatch > 1)
                {
                    sb.Append(currentBatch);
                }

                currentChar = c;
                currentBatch = 1;
            }
            else
            {
                currentBatch++;
            }
        }

        if (currentChar != null && currentBatch != null)
        {
            sb.Append(currentChar);
            if (currentBatch > 1)
            {
                sb.Append(currentBatch);
            }
        }

        return sb;
    }

    public static StringBuilder DecompressString(string compressedString)
    {
        if (compressedString.Length == 0)
        {
            return new StringBuilder();
        }
        var sb = new StringBuilder();

        var currentNumber = "";
        char? currentLetter = null;
        foreach (var c in compressedString)
        {
            if (CharConsistency.IsCharNumber(c))
            {
                currentNumber += c;
            }
            else if (CharConsistency.IsCharLowerCaseLetter(c))
            {
                if (currentLetter == null)
                {
                    currentLetter = c;
                }
                else if (currentNumber == "")
                {
                    sb.Append(currentLetter);
                    currentLetter = c;
                } 
                else
                {
                    AddRepeatedLetterToSb((char)currentLetter, currentNumber, ref sb);
                    currentLetter = c;
                    currentNumber = "";
                }
            }
        }

        currentNumber = currentNumber == "" ? "1" : currentNumber;
        AddRepeatedLetterToSb((char)currentLetter!, currentNumber, ref sb);

        return sb;
    }
    
    private static void AddRepeatedLetterToSb(char letter, string currentNumber, ref StringBuilder sb)
    {
        if (!int.TryParse(currentNumber, out var repeatedCount))
        {
            throw new IncorrectCompressedDataException("Number has incorrect format");
        }

        if (repeatedCount < 1)
        {
            throw new IncorrectCompressedDataException("Number is less than 1");
        }
                    
        sb.Append(letter, repeatedCount);
    }
}