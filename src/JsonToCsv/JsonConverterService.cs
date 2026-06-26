// Orchestrates JSON-to-CSV conversion: validates input, parses with JsonDocument,
// flattens records, and writes output atomically — deleting any partial output on failure.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace JsonToCsv;

public static class JsonConverterService
{
    public static void Convert(string inputPath, string outputPath, char delimiter)
    {
        if (!File.Exists(inputPath))
            throw new FileNotFoundException($"Input file not found: '{inputPath}'", inputPath);

        string jsonContent = File.ReadAllText(inputPath);

        using var document = JsonDocument.Parse(jsonContent);
        var root = document.RootElement;

        List<Dictionary<string, string>> records;

        if (root.ValueKind == JsonValueKind.Object)
        {
            records = new List<Dictionary<string, string>>
            {
                JsonFlattener.Flatten(root)
            };
        }
        else if (root.ValueKind == JsonValueKind.Array)
        {
            records = new List<Dictionary<string, string>>();
            foreach (var element in root.EnumerateArray())
            {
                if (element.ValueKind != JsonValueKind.Object)
                    throw new InvalidOperationException(
                        "JSON array must contain only objects. Found a non-object element.");

                records.Add(JsonFlattener.Flatten(element));
            }
        }
        else
        {
            throw new InvalidOperationException(
                $"Root JSON element must be an Object or Array of Objects, but got '{root.ValueKind}'.");
        }

        var headers = records
            .SelectMany(r => r.Keys)
            .Distinct()
            .ToList();

        try
        {
            using var writer = new StreamWriter(outputPath, append: false);
            CsvWriter.Write(records, headers, writer, delimiter);
        }
        catch
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);
            throw;
        }
    }
}
