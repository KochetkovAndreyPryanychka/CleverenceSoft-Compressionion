using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;
using Compression.Exceptions;

namespace Compression;

internal class Program
{
    private const int MbSize = 1024 * 1024;                                                             // Size of data block

    private static void Main()
    {
        var configuration = Configure();
        var maxParallelismDegree = GetMaxParallelismDegree(configuration);

        var inputString = Console.ReadLine()!;

        var blocks = SplitStringToBlocks(inputString).ToArray();

        var stringBuilders = CompressAllBlocks(blocks, maxParallelismDegree);
        
        Console.WriteLine(GetResult(stringBuilders));
    }

    private static IEnumerable<string> SplitStringToBlocks(string inputString, int blockSize = MbSize)
    {
        var i = 0;
        while(i < inputString.Length) {
            var lengthToTake = Math.Min(blockSize, inputString.Length - i);
            if (i + lengthToTake < inputString.Length && inputString[i + lengthToTake - 1] == inputString[i + lengthToTake])
            {
                var lastChar = inputString[i + lengthToTake - 1];
                for (lengthToTake += 1; i + lengthToTake < inputString.Length; lengthToTake++)
                {
                    if (inputString[i + lengthToTake] != lastChar)
                    {
                        break;
                    }
                }
            }

            var beginOfBlock = i;
            i += lengthToTake;
            yield return inputString.Substring(beginOfBlock, lengthToTake);
        }
    }

    private static StringBuilder CompressString(string inputString)
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

    private static StringBuilder GetResult(IEnumerable<StringBuilder> stringBuilders)
    {
        var finalSb = new StringBuilder();
        foreach (var sb in stringBuilders)
        {
            finalSb.Append(sb);
        }

        return finalSb;
    }

    private static IEnumerable<StringBuilder> CompressAllBlocks(IReadOnlyList<string> blocks, int maxParallelismDegree)
    {
        var stringBuilders = new StringBuilder[blocks.Count];
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = maxParallelismDegree };
        Parallel.For(0, blocks.Count, parallelOptions, i =>
        {
            stringBuilders[i] = CompressString(blocks[i]);
        });

        return stringBuilders;
    }

    private static IConfiguration Configure()
    {
        var rootPath = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent!.Parent!.Parent!.FullName;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(rootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        return configuration;
    }

    private static int GetMaxParallelismDegree(IConfiguration configuration)
    {
        if (!int.TryParse(configuration["MultiThreading:MaxParallelismDegree"]!, out var maxParallelismDegree) ||
            maxParallelismDegree < 0)
        {
            throw new InvalidAppSettingsException("Invalid MultiThreading:MaxParallelismDegree parameter");
        };

        return maxParallelismDegree;
    }
}