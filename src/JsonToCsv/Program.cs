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
                if (!opts.TryResolveDelimiter(out _))
                {
                    Console.Error.WriteLine("Error: delimiter must resolve to exactly one character.");
                    exitCode = 1;
                    return;
                }

                try
                {
                    if (!File.Exists(opts.Input))
                    {
                        Console.Error.WriteLine($"Error: input file '{opts.Input}' not found.");
                        exitCode = 1;
                        return;
                    }

                    exitCode = 0;
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
