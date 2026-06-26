using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace JsonToCsv
{
    public class JsonFlattenerTests
    {
        [Fact]
        public void TestAcceptanceExample_NestedProfile()
        {
            var json = "{\"id\": 1, \"profile\": {\"age\": 30, \"location\": {\"city\": \"Berlin\"}}}";
            using var doc = JsonDocument.Parse(json);
            var result = JsonFlattener.Flatten(doc.RootElement);
            Assert.Equal("1", result["id"]);
            Assert.Equal("30", result["profile.age"]);
            Assert.Equal("Berlin", result["profile.location.city"]);
        }

        [Fact]
        public void TestAcceptanceExample_PrimitiveArray()
        {
            var json = "{\"id\": 1, \"tags\": [\"admin\", \"user\"]}";
            using var doc = JsonDocument.Parse(json);
            var result = JsonFlattener.Flatten(doc.RootElement);
            Assert.Equal("1", result["id"]);
            Assert.Equal("[\"admin\",\"user\"]", result["tags"]);
        }

        [Fact]
        public void TestAcceptanceExample_RootLevelEmptyCollection()
        {
            var json = "{}";
            using var doc = JsonDocument.Parse(json);
            var result = JsonFlattener.Flatten(doc.RootElement);
            Assert.Single(result);
            Assert.Equal("", result[""]);
        }

        [Fact]
        public void TestAcceptanceExample_NestedEmptyAndNull()
        {
            var json = "{\"nested\": {}, \"field\": null}";
            using var doc = JsonDocument.Parse(json);
            var result = JsonFlattener.Flatten(doc.RootElement);
            Assert.Equal(2, result.Count);
            Assert.Equal("", result["nested"]);
            Assert.Equal("", result["field"]);
        }

        [Fact]
        public void TestAcceptanceExample_ExceedingDepthLimit()
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < 101; i++)
            {
                sb.Append("{\"a\":");
            }
            sb.Append("1");
            for (int i = 0; i < 101; i++)
            {
                sb.Append("}");
            }
            var options = new JsonDocumentOptions { MaxDepth = 120 };
            using var doc = JsonDocument.Parse(sb.ToString(), options);
            Assert.Throws<ArgumentException>(() => JsonFlattener.Flatten(doc.RootElement));
        }

        [Fact]
        public void TestAcceptanceExample_NestedObjectsInArray()
        {
            var json = "{\"tags\": [{\"name\": \"admin\"}]}";
            using var doc = JsonDocument.Parse(json);
            Assert.Throws<ArgumentException>(() => JsonFlattener.Flatten(doc.RootElement));
        }

        [Theory]
        [InlineData("true", "true")]
        [InlineData("false", "false")]
        public void TestBooleans(string jsonVal, string expected)
        {
            var json = "{\"val\": " + jsonVal + "}";
            using var doc = JsonDocument.Parse(json);
            var result = JsonFlattener.Flatten(doc.RootElement);
            Assert.Equal(expected, result["val"]);
        }

        [Theory]
        [InlineData("123", "123")]
        [InlineData("12.34", "12.34")]
        [InlineData("-0.005", "-0.005")]
        public void TestNumbers(string jsonVal, string expected)
        {
            var json = "{\"num\": " + jsonVal + "}";
            using var doc = JsonDocument.Parse(json);
            var result = JsonFlattener.Flatten(doc.RootElement);
            Assert.Equal(expected, result["num"]);
        }

        [Fact]
        public void TestPrimitiveArrayWithDifferentTypes()
        {
            var json = "{\"mixed\": [1, \"two\", true, null]}";
            using var doc = JsonDocument.Parse(json);
            var result = JsonFlattener.Flatten(doc.RootElement);
            Assert.Equal("[1,\"two\",true,null]", result["mixed"]);
        }

        [Fact]
        public void TestArrayOfArraysThrows()
        {
            var json = "{\"nested_arr\": [[1, 2]]}";
            using var doc = JsonDocument.Parse(json);
            Assert.Throws<ArgumentException>(() => JsonFlattener.Flatten(doc.RootElement));
        }

        [Fact]
        public void TestDeepNestingExactlyAtBoundary()
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                sb.Append("{\"a\":");
            }
            sb.Append("1");
            for (int i = 0; i < 100; i++)
            {
                sb.Append("}");
            }
            var options = new JsonDocumentOptions { MaxDepth = 120 };
            using var doc = JsonDocument.Parse(sb.ToString(), options);
            var result = JsonFlattener.Flatten(doc.RootElement);
            Assert.NotNull(result);
        }

        [Fact]
        public void TestCustomPrefix()
        {
            var json = "{\"val\": 42}";
            using var doc = JsonDocument.Parse(json);
            var result = JsonFlattener.Flatten(doc.RootElement, "custom_prefix");
            Assert.Equal("42", result["custom_prefix.val"]);
        }
    }
}
