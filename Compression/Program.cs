using Microsoft.Extensions.Configuration;
using Compression.Exceptions;

namespace Compression;

internal class Program
{
    private const double AverageRleCompressionCoefficient = 2;

    private static void Main()
    {
        var configuration = Configure();
        var maxParallelismDegree = GetMaxParallelismDegree(configuration);

        var inputString = Console.ReadLine()!;

        var compressUtility = new CompressionUtility(
            inputString,
            CompressionFunctions.CompressString, 
            maxParallelismDegree, 
            coefficient: 1);
        var compressedString = compressUtility.Run().ToString();
        Console.WriteLine(compressedString);

        var decompressUtility = new CompressionUtility(
            compressedString,
            CompressionFunctions.DecompressString,
            maxParallelismDegree,
            coefficient: AverageRleCompressionCoefficient);
        var decompressString = decompressUtility.Run().ToString();
        Console.WriteLine(decompressString);
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