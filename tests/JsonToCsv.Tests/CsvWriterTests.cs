using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using JsonToCsv;

public class CsvWriterTests
{
    [Fact]
    public void Write_AcceptanceExample1_ProducesCorrectOutput()
    {
        var records = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { { "a", "1" } },
            new Dictionary<string, string> { { "b", "2" } }
        };
        var headers = new List<string> { "a", "b" };
        using var writer = new StringWriter();
        CsvWriter.Write(records, headers, writer, ',');

        var expected = "a,b\r\n1,\r\n,2\r\n";
        Assert.Equal(expected, writer.ToString());
    }

    [Fact]
    public void Write_AcceptanceExample2_ProducesCorrectOutput()
    {
        var records = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string>
            {
                { "data", "user;active" },
                { "quotes", "Alice \"The Boss\"" }
            }
        };
        var headers = new List<string> { "data", "quotes" };
        using var writer = new StringWriter();
        CsvWriter.Write(records, headers, writer, ';');

        var expected = "data;quotes\r\n\"user;active\";\"Alice \"\"The Boss\"\"\"\r\n";
        Assert.Equal(expected, writer.ToString());
    }

    [Fact]
    public void Write_RecordsNull_ThrowsArgumentNullException()
    {
        var headers = new List<string> { "a" };
        using var writer = new StringWriter();
        Assert.Throws<ArgumentNullException>(() => CsvWriter.Write(null!, headers, writer, ','));
    }

    [Fact]
    public void Write_HeadersNull_ThrowsArgumentNullException()
    {
        var records = new List<Dictionary<string, string>>();
        using var writer = new StringWriter();
        Assert.Throws<ArgumentNullException>(() => CsvWriter.Write(records, null!, writer, ','));
    }

    [Fact]
    public void Write_TargetWriterNull_ThrowsArgumentNullException()
    {
        var records = new List<Dictionary<string, string>>();
        var headers = new List<string> { "a" };
        Assert.Throws<ArgumentNullException>(() => CsvWriter.Write(records, headers, null!, ','));
    }

    [Fact]
    public void Write_NullRecordInCollection_ThrowsArgumentNullException()
    {
        var records = new List<Dictionary<string, string>> { null! };
        var headers = new List<string> { "a" };
        using var writer = new StringWriter();
        Assert.Throws<ArgumentNullException>(() => CsvWriter.Write(records, headers, writer, ','));
    }

    [Fact]
    public void Write_EmptyRecords_WritesOnlyHeaders()
    {
        var records = new List<Dictionary<string, string>>();
        var headers = new List<string> { "a", "b" };
        using var writer = new StringWriter();
        CsvWriter.Write(records, headers, writer, ',');

        Assert.Equal("a,b\r\n", writer.ToString());
    }

    [Fact]
    public void Write_MissingKeysInRecords_OutputsEmptyValues()
    {
        var records = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { { "a", "valA" } },
            new Dictionary<string, string> { { "b", "valB" } },
            new Dictionary<string, string>()
        };
        var headers = new List<string> { "a", "b" };
        using var writer = new StringWriter();
        CsvWriter.Write(records, headers, writer, ',');

        var expected = "a,b\r\nvalA,\r\n,valB\r\n,\r\n";
        Assert.Equal(expected, writer.ToString());
    }

    [Theory]
    [InlineData("val,with,delimiter", ',', "\"val,with,delimiter\"")]
    [InlineData("val\"with\"quotes", ',', "\"val\"\"with\"\"quotes\"")]
    [InlineData("val\nwith\nnewlines", ',', "\"val\nwith\nnewlines\"")]
    [InlineData("val\rwith\rcarriage", ',', "\"val\rwith\rcarriage\"")]
    [InlineData("normal", ',', "normal")]
    public void Write_EscapingBehavior_CorrectlyQuotesWhenNecessary(string value, char delimiter, string expectedOutput)
    {
        var records = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { { "col", value } }
        };
        var headers = new List<string> { "col" };
        using var writer = new StringWriter();
        CsvWriter.Write(records, headers, writer, delimiter);

        var expected = $"col\r\n{expectedOutput}\r\n";
        Assert.Equal(expected, writer.ToString());
    }

    [Fact]
    public void Write_HeadersNeedEscaping_QuotesHeadersCorrectly()
    {
        var records = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { { "header,1", "val1" }, { "header\"2", "val2" } }
        };
        var headers = new List<string> { "header,1", "header\"2" };
        using var writer = new StringWriter();
        CsvWriter.Write(records, headers, writer, ',');

        var expected = "\"header,1\",\"header\"\"2\"\r\nval1,val2\r\n";
        Assert.Equal(expected, writer.ToString());
    }

    [Fact]
    public void Write_WithNullDictionaryValue_TreatsAsEmptyString()
    {
        var records = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { { "a", null! } }
        };
        var headers = new List<string> { "a" };
        using var writer = new StringWriter();
        CsvWriter.Write(records, headers, writer, ',');

        Assert.Equal("a\r\n\r\n", writer.ToString());
    }

    [Fact]
    public void Write_WhenWriterThrows_PropagatesIOException()
    {
        var records = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { { "a", "1" } }
        };
        var headers = new List<string> { "a" };
        using var writer = new FaultyTextWriter();
        Assert.Throws<IOException>(() => CsvWriter.Write(records, headers, writer, ','));
    }

    private class FaultyTextWriter : TextWriter
    {
        public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;
        public override void Write(string? value) => throw new IOException("Simulated write failure");
        public override void Write(char value) => throw new IOException("Simulated write failure");
    }
}
