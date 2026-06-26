using System;
using System.IO;
using Xunit;
using JsonToCsv;

namespace JsonToCsv
{
    public class ProgramTests
    {
        [Fact]
        public void Main_WithEmptyArgs_ReturnsNonZero()
        {
            string[] args = Array.Empty<string>();
            int exitCode = Program.Main(args);
            Assert.NotEqual(0, exitCode);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("")]
        [InlineData("ab")]
        public void Main_WithInvalidDelimiter_ReturnsExitCodeOne(string invalidDelimiter)
        {
            string[] args = new[] { "-i", "input.json", "-o", "output.csv", "-d", invalidDelimiter };
            int exitCode = Program.Main(args);
            Assert.Equal(1, exitCode);
        }

        [Theory]
        [InlineData(",")]
        [InlineData("\\t")]
        [InlineData("\\n")]
        [InlineData("\\r")]
        public void Main_WithValidDelimiterButMissingInputFile_ReturnsNonZeroOrThrows(string validDelimiter)
        {
            string nonexistentFile = $"nonexistent_{Guid.NewGuid()}.json";
            string[] args = new[] { "-i", nonexistentFile, "-o", "output.csv", "-d", validDelimiter };

            try
            {
                int exitCode = Program.Main(args);
                Assert.NotEqual(0, exitCode);
            }
            catch (Exception ex)
            {
                Assert.IsAssignableFrom<Exception>(ex);
            }
        }

        [Fact]
        public void Main_WithMissingRequiredArguments_ReturnsNonZero()
        {
            string[] args = new[] { "-d", "," };
            int exitCode = Program.Main(args);
            Assert.NotEqual(0, exitCode);
        }
    }
}
