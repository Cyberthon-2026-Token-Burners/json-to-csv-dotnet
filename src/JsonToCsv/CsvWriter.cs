// Stateless CSV serializer: writes flattened records to a TextWriter with custom delimiter and RFC 4180-compliant escaping.

using System;
using System.Collections.Generic;
using System.IO;

namespace JsonToCsv;

public static class CsvWriter
{
    public static void Write(
        IEnumerable<Dictionary<string, string>> records,
        IEnumerable<string> headers,
        TextWriter targetWriter,
        char delimiter)
    {
        ArgumentNullException.ThrowIfNull(records);
        ArgumentNullException.ThrowIfNull(headers);
        ArgumentNullException.ThrowIfNull(targetWriter);

        var headerList = new List<string>(headers);

        // Write header row
        for (int i = 0; i < headerList.Count; i++)
        {
            if (i > 0) targetWriter.Write(delimiter);
            targetWriter.Write(EscapeField(headerList[i], delimiter));
        }
        targetWriter.Write("\r\n");

        // Write data rows
        foreach (var record in records)
        {
            if (record is null)
                throw new ArgumentNullException(nameof(records), "A record in the records enumerable is null.");

            for (int i = 0; i < headerList.Count; i++)
            {
                if (i > 0) targetWriter.Write(delimiter);

                if (record.TryGetValue(headerList[i], out var value) && value is not null)
                    targetWriter.Write(EscapeField(value, delimiter));
                // else: empty field — write nothing (no quotes)
            }
            targetWriter.Write("\r\n");
        }
    }

    private static string EscapeField(string field, char delimiter)
    {
        bool needsQuoting = field.Contains(delimiter)
            || field.Contains('"')
            || field.Contains('\r')
            || field.Contains('\n');

        if (!needsQuoting)
            return field;

        return "\"" + field.Replace("\"", "\"\"") + "\"";
    }
}
