using System.Collections.Generic;
using System.Text.Json;

namespace JsonToCsv
{
    public static class JsonFlattener
    {
        public static Dictionary<string, string> Flatten(JsonElement element, string prefix = "")
        {
            return FlattenInternal(element, prefix, 0);
        }

        private static Dictionary<string, string> FlattenInternal(JsonElement element, string prefix, int depth)
        {
            if (depth > 100)
                throw new System.ArgumentException($"Recursion depth exceeded maximum of 100 levels at prefix '{prefix}'.");

            var result = new Dictionary<string, string>();

            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    bool hasProperties = false;
                    foreach (var property in element.EnumerateObject())
                    {
                        hasProperties = true;
                        string childKey = string.IsNullOrEmpty(prefix)
                            ? property.Name
                            : $"{prefix}.{property.Name}";
                        foreach (var kv in FlattenInternal(property.Value, childKey, depth + 1))
                            result[kv.Key] = kv.Value;
                    }
                    if (!hasProperties)
                        result[prefix] = "";
                    break;

                case JsonValueKind.Array:
                    bool hasElements = false;
                    bool hasNonPrimitiveElements = false;

                    foreach (var item in element.EnumerateArray())
                    {
                        hasElements = true;
                        if (item.ValueKind == JsonValueKind.Object || item.ValueKind == JsonValueKind.Array)
                            hasNonPrimitiveElements = true;
                    }

                    if (!hasElements)
                    {
                        result[prefix] = "";
                    }
                    else if (hasNonPrimitiveElements)
                    {
                        throw new System.ArgumentException(
                            $"Non-primitive array elements are not supported at prefix '{prefix}'.");
                    }
                    else
                    {
                        // Primitive array: serialize each element compactly and join
                        var parts = new List<string>();
                        foreach (var item in element.EnumerateArray())
                            parts.Add(JsonSerializer.Serialize(item));
                        result[prefix] = "[" + string.Join(",", parts) + "]";
                    }
                    break;

                case JsonValueKind.String:
                    result[prefix] = element.GetString() ?? "";
                    break;

                case JsonValueKind.Number:
                    result[prefix] = element.GetRawText();
                    break;

                case JsonValueKind.True:
                    result[prefix] = "true";
                    break;

                case JsonValueKind.False:
                    result[prefix] = "false";
                    break;

                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                default:
                    result[prefix] = "";
                    break;
            }

            return result;
        }
    }
}
