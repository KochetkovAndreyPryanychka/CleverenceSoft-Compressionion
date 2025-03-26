using System.Text;

namespace Compression;

public class CompressionUtility
{
    private const int MbSize = 4; // Size of data block

    private readonly int _blockSize;

    private readonly Func<string, StringBuilder> _utilityFunc;

    private readonly string _sourceString;

    private readonly int _maxParallelismDegree;

    public CompressionUtility(
        string sourceString, 
        Func<string, StringBuilder> func,
        int maxParallelismDegree,
        double coefficient,
        int blockSize = MbSize)
    {
        _sourceString = sourceString;
        _utilityFunc = func;
        _maxParallelismDegree = maxParallelismDegree;
        _blockSize = (int)(blockSize / coefficient);
    }

    public StringBuilder Run()
    {
        var blocks = SplitStringToBlocks(_sourceString).ToArray();
        var stringBuilders = GetResultFromAllBlocks(blocks, _maxParallelismDegree);
        return GetResult(stringBuilders);
    }
    
    private IEnumerable<string> SplitStringToBlocks(string inputString)
    {
        var i = 0;
        while(i < inputString.Length)
        {
            var lengthToTake = GetLengthToTake(inputString, i);
            
            var beginOfBlock = i;
            i += lengthToTake;
            yield return inputString.Substring(beginOfBlock, lengthToTake);
        }
    }

    private int GetLengthToTake(string str, int i)
    {
        var lengthToTake = Math.Min(_blockSize, str.Length - i);
        if (i + lengthToTake < str.Length && 
            (str[i + lengthToTake - 1] == str[i + lengthToTake] && 
             CharConsistency.IsCharLowerCaseLetter(str[i + lengthToTake - 1]) ||
             CharConsistency.IsCharNumber(str[i + lengthToTake - 1]) &&
             CharConsistency.IsCharNumber(str[i + lengthToTake])))
        {
            if (CharConsistency.IsCharNumber(str[i + lengthToTake - 1]))
            {
                for (lengthToTake += 1; i + lengthToTake < str.Length; lengthToTake++)
                {
                    if (!CharConsistency.IsCharNumber(str[i + lengthToTake]))
                    {
                        break;
                    }
                }
            }
            else
            {
                var lastChar = str[i + lengthToTake - 1];
                for (lengthToTake += 1; i + lengthToTake < str.Length; lengthToTake++)
                {
                    if (str[i + lengthToTake] != lastChar)
                    {
                        break;
                    }
                }
            }
        }

        return lengthToTake;
    }

    private static StringBuilder GetResult(IEnumerable<StringBuilder> stringBuilders)
    {
        var finalSb = new StringBuilder();
        foreach (var sb in stringBuilders)
        {
            finalSb.Append(sb);
        }

        return finalSb;
    }
    
    private IEnumerable<StringBuilder> GetResultFromAllBlocks(IReadOnlyList<string> blocks, int maxParallelismDegree)
    {
        var stringBuilders = new StringBuilder[blocks.Count];
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = maxParallelismDegree };
        Parallel.For(0, blocks.Count, parallelOptions, i =>
        {
            stringBuilders[i] = _utilityFunc(blocks[i]);
        });

        return stringBuilders;
    }
}