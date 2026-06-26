using System;
using System.IO;
using System.Text.Json;
using CommandLine;

namespace JsonToCsv;

public class Program
{
    public static int Main(string[] args)
    {
        int exitCode = 1;

        using var parser = new Parser(config => { config.HelpWriter = Console.Error; });

        parser.ParseArguments<CliOptions>(args)
            .WithParsed(opts =>
            {
                if (!opts.TryResolveDelimiter(out char delimiter))
                {
                    Console.Error.WriteLine("Error: delimiter must resolve to exactly one character.");
                    exitCode = 1;
                    return;
                }

                try
                {
                    JsonConverterService.Convert(opts.Input, opts.Output, delimiter);
                    exitCode = 0;
                }
                catch (JsonException ex)
                {
                    Console.Error.WriteLine($"Error: JSON parsing failed at line {ex.LineNumber}, column {ex.BytePositionInLine}. Details: {ex.Message}");
                    exitCode = 1;
                }
                catch (FileNotFoundException ex)
                {
                    Console.Error.WriteLine($"Error: {ex.Message}");
                    exitCode = 1;
                }
                catch (InvalidOperationException ex)
                {
                    Console.Error.WriteLine($"Error: {ex.Message}");
                    exitCode = 1;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error: {ex.Message}");
                    exitCode = 1;
                }
            })
            .WithNotParsed(_ => { exitCode = 1; });

        return exitCode;
    }
}
