using System;
using System.IO;
using System.Text.Json;
using Xunit;
using JsonToCsv;

namespace JsonToCsv
{
    public class JsonConverterServiceTests
    {
        [Fact]
        public void Convert_WithNonexistentInputFile_ThrowsFileNotFoundException()
        {
            string nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");
            string outputFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".csv");

            try
            {
                Assert.Throws<FileNotFoundException>(() =>
                    JsonConverterService.Convert(nonExistentFile, outputFile, ',')
                );
            }
            finally
            {
                if (File.Exists(outputFile)) File.Delete(outputFile);
            }
        }

        [Theory]
        [InlineData("{")]
        [InlineData("{\"name\": \"value\"")]
        [InlineData("[{\"name\": \"value\"}")]
        [InlineData("invalid json")]
        public void Convert_WithMalformedJson_ThrowsJsonException(string malformedJson)
        {
            string inputFile = Path.GetTempFileName();
            string outputFile = Path.GetTempFileName();

            try
            {
                File.WriteAllText(inputFile, malformedJson);
                Assert.ThrowsAny<JsonException>(() =>
                    JsonConverterService.Convert(inputFile, outputFile, ',')
                );
            }
            finally
            {
                if (File.Exists(inputFile)) File.Delete(inputFile);
                if (File.Exists(outputFile)) File.Delete(outputFile);
            }
        }

        [Theory]
        [InlineData("[\"apple\",\"banana\"]")]
        [InlineData("[1, 2, 3]")]
        [InlineData("[true, false]")]
        [InlineData("\"string-root\"")]
        [InlineData("42")]
        [InlineData("true")]
        [InlineData("null")]
        public void Convert_WithPrimitiveArrayOrPrimitiveRoot_ThrowsInvalidOperationException(string invalidJson)
        {
            string inputFile = Path.GetTempFileName();
            string outputFile = Path.GetTempFileName();

            try
            {
                File.WriteAllText(inputFile, invalidJson);
                Assert.Throws<InvalidOperationException>(() =>
                    JsonConverterService.Convert(inputFile, outputFile, ',')
                );
            }
            finally
            {
                if (File.Exists(inputFile)) File.Delete(inputFile);
                if (File.Exists(outputFile)) File.Delete(outputFile);
            }
        }

        [Theory]
        [InlineData("[{\"a\": 1}, \"invalid-element\", {\"b\": 2}]")]
        [InlineData("[{\"a\": 1}, 100]")]
        public void Convert_WithArrayContainingNonObjectElements_ThrowsInvalidOperationException(string json)
        {
            string inputFile = Path.GetTempFileName();
            string outputFile = Path.GetTempFileName();

            try
            {
                File.WriteAllText(inputFile, json);
                Assert.Throws<InvalidOperationException>(() =>
                    JsonConverterService.Convert(inputFile, outputFile, ',')
                );
            }
            finally
            {
                if (File.Exists(inputFile)) File.Delete(inputFile);
                if (File.Exists(outputFile)) File.Delete(outputFile);
            }
        }

        [Fact]
        public void Convert_WithValidJsonArray_ConvertsToCsvSuccessfully()
        {
            string json = "[\n  {\"id\": 1, \"name\": \"Alice\", \"details\": {\"role\": \"Admin\", \"active\": true}},\n  {\"id\": 2, \"name\": \"Bob\", \"details\": {\"role\": \"User\", \"active\": false}}\n]";
            string inputFile = Path.GetTempFileName();
            string outputFile = Path.GetTempFileName();

            try
            {
                File.WriteAllText(inputFile, json);
                JsonConverterService.Convert(inputFile, outputFile, ',');
                string csvContent = File.ReadAllText(outputFile);

                Assert.NotEmpty(csvContent);
                Assert.Contains("id", csvContent);
                Assert.Contains("name", csvContent);
                Assert.Contains("details.role", csvContent);
                Assert.Contains("details.active", csvContent);
                Assert.Contains("Alice", csvContent);
                Assert.Contains("Admin", csvContent);
                Assert.Contains("Bob", csvContent);
                Assert.Contains("User", csvContent);
            }
            finally
            {
                if (File.Exists(inputFile)) File.Delete(inputFile);
                if (File.Exists(outputFile)) File.Delete(outputFile);
            }
        }

        [Fact]
        public void Convert_WithCustomDelimiter_UsesDelimiterInOutput()
        {
            string json = "[\n  {\"name\": \"Alice\", \"city\": \"Paris\"},\n  {\"name\": \"Bob\", \"city\": \"London\"}\n]";
            string inputFile = Path.GetTempFileName();
            string outputFile = Path.GetTempFileName();
            char customDelimiter = ';';

            try
            {
                File.WriteAllText(inputFile, json);
                JsonConverterService.Convert(inputFile, outputFile, customDelimiter);
                string csvContent = File.ReadAllText(outputFile);

                Assert.NotEmpty(csvContent);
                Assert.Contains("name;city", csvContent);
                Assert.Contains("Alice;Paris", csvContent);
                Assert.Contains("Bob;London", csvContent);
            }
            finally
            {
                if (File.Exists(inputFile)) File.Delete(inputFile);
                if (File.Exists(outputFile)) File.Delete(outputFile);
            }
        }
    }
}
