using System;
using Xunit;
using JsonToCsv;

namespace JsonToCsv
{
    public class CliOptionsTests
    {
        [Theory]
        [InlineData(",", true, ',')]
        [InlineData("\t", true, '\t')]
        [InlineData("\\t", true, '\t')]
        [InlineData("\n", true, '\n')]
        [InlineData("\\n", true, '\n')]
        [InlineData("\r", true, '\r')]
        [InlineData("\\r", true, '\r')]
        [InlineData("a", true, 'a')]
        [InlineData(" ", true, ' ')]
        [InlineData("", false, '\0')]
        [InlineData("abc", false, '\0')]
        [InlineData("\\abc", false, '\0')]
        [InlineData(null, false, '\0')]
        public void TestTryResolveDelimiter(string? delimiterInput, bool expectedSuccess, char expectedChar)
        {
            var options = new CliOptions
            {
                Input = "dummy.json",
                Output = "dummy.csv",
                Delimiter = delimiterInput!
            };

            bool success = options.TryResolveDelimiter(out char resolved);

            Assert.Equal(expectedSuccess, success);
            Assert.Equal(expectedChar, resolved);
        }

        [Fact]
        public void TestDefaultDelimiterValue()
        {
            var options = new CliOptions();
            Assert.Equal(",", options.Delimiter);
            bool success = options.TryResolveDelimiter(out char resolved);
            Assert.True(success);
            Assert.Equal(',', resolved);
        }

        [Fact]
        public void TestPropertyAccessors()
        {
            var options = new CliOptions
            {
                Input = "test_input.json",
                Output = "test_output.csv",
                Delimiter = "|"
            };

            Assert.Equal("test_input.json", options.Input);
            Assert.Equal("test_output.csv", options.Output);
            Assert.Equal("|", options.Delimiter);
        }
    }
}
