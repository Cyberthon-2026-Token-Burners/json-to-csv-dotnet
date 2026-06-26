using CommandLine;

namespace JsonToCsv;

[Verb("run", HelpText = "Converts JSON to CSV")]
public class CliOptions
{
    [Option('i', "input", Required = true, HelpText = "Path to input JSON file.")]
    public string Input { get; set; } = string.Empty;

    [Option('o', "output", Required = true, HelpText = "Path to output CSV file.")]
    public string Output { get; set; } = string.Empty;

    [Option('d', "delimiter", Required = false, Default = ",", HelpText = "Custom separator character.")]
    public string Delimiter { get; set; } = ",";

    public bool TryResolveDelimiter(out char resolvedDelimiter)
    {
        resolvedDelimiter = '\0';

        if (Delimiter is null || Delimiter.Length == 0)
            return false;

        // Interpret two-char escape sequences: \t, \n, \r
        if (Delimiter.Length == 2 && Delimiter[0] == '\\')
        {
            switch (Delimiter[1])
            {
                case 't':
                    resolvedDelimiter = '\t';
                    return true;
                case 'n':
                    resolvedDelimiter = '\n';
                    return true;
                case 'r':
                    resolvedDelimiter = '\r';
                    return true;
                default:
                    return false;
            }
        }

        if (Delimiter.Length != 1)
            return false;

        resolvedDelimiter = Delimiter[0];
        return true;
    }
}
