using System;
using System.IO;
using Xunit;
using JsonToCsv;

namespace JsonToCsv
{
    public class ProgramTests
    {
        [Fact]
        public void Main_WithEmptyArgs_ReturnsExitCodeOne()
        {
            string[] args = Array.Empty<string>();
            int exitCode = Program.Main(args);
            Assert.Equal(1, exitCode);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("")]
        [InlineData("ab")]
        [InlineData("123")]
        public void Main_WithInvalidDelimiter_ReturnsExitCodeOne(string invalidDelimiter)
        {
            string[] args = new[] { "-i", "input.json", "-o", "output.csv", "-d", invalidDelimiter };
            int exitCode = Program.Main(args);
            Assert.Equal(1, exitCode);
        }

        [Theory]
        [InlineData("-i", "input.json")]
        [InlineData("-o", "output.csv")]
        [InlineData("-d", ",")]
        [InlineData("-i", "input.json", "-d", ",")]
        [InlineData("-o", "output.csv", "-d", ",")]
        public void Main_WithMissingRequiredArguments_ReturnsExitCodeOne(params string[] args)
        {
            int exitCode = Program.Main(args);
            Assert.Equal(1, exitCode);
        }

        [Fact]
        public void Main_WithNonexistentInputFile_ReturnsExitCodeOne()
        {
            string nonexistentFile = Path.Combine(Path.GetTempPath(), $"nonexistent_{Guid.NewGuid()}.json");
            string outputFile = Path.Combine(Path.GetTempPath(), $"output_{Guid.NewGuid()}.csv");

            string[] args = new[] { "-i", nonexistentFile, "-o", outputFile };
            int exitCode = Program.Main(args);

            Assert.Equal(1, exitCode);
        }

        [Fact]
        public void Main_WithMalformedJson_ReturnsExitCodeOne()
        {
            string inputFile = Path.GetTempFileName();
            string outputFile = Path.Combine(Path.GetTempPath(), $"output_{Guid.NewGuid()}.csv");

            try
            {
                File.WriteAllText(inputFile, "{ \"invalid\": json ");
                string[] args = new[] { "-i", inputFile, "-o", outputFile };
                int exitCode = Program.Main(args);
                Assert.Equal(1, exitCode);
            }
            finally
            {
                if (File.Exists(inputFile)) File.Delete(inputFile);
                if (File.Exists(outputFile)) File.Delete(outputFile);
            }
        }

        [Fact]
        public void Main_WithPrimitiveArrayJson_ReturnsExitCodeOne()
        {
            string inputFile = Path.GetTempFileName();
            string outputFile = Path.Combine(Path.GetTempPath(), $"output_{Guid.NewGuid()}.csv");

            try
            {
                File.WriteAllText(inputFile, "[\"apple\", \"banana\"]");
                string[] args = new[] { "-i", inputFile, "-o", outputFile };
                int exitCode = Program.Main(args);
                Assert.Equal(1, exitCode);
            }
            finally
            {
                if (File.Exists(inputFile)) File.Delete(inputFile);
                if (File.Exists(outputFile)) File.Delete(outputFile);
            }
        }

        [Theory]
        [InlineData(",")]
        [InlineData("\\t")]
        public void Main_WithValidJsonAndDelimiter_ConvertsSuccessfully(string delimiterOption)
        {
            string inputFile = Path.GetTempFileName();
            string outputFile = Path.Combine(Path.GetTempPath(), $"output_{Guid.NewGuid()}.csv");

            try
            {
                string jsonContent = "[{\"name\":\"Alice\",\"age\":30},{\"name\":\"Bob\",\"age\":25}]";
                File.WriteAllText(inputFile, jsonContent);

                string[] args = new[] { "-i", inputFile, "-o", outputFile, "-d", delimiterOption };
                int exitCode = Program.Main(args);

                Assert.Equal(0, exitCode);
                Assert.True(File.Exists(outputFile));

                string csvContent = File.ReadAllText(outputFile);
                Assert.Contains("name", csvContent);
                Assert.Contains("age", csvContent);
                Assert.Contains("Alice", csvContent);
                Assert.Contains("Bob", csvContent);
            }
            finally
            {
                if (File.Exists(inputFile)) File.Delete(inputFile);
                if (File.Exists(outputFile)) File.Delete(outputFile);
            }
        }
    }
}
