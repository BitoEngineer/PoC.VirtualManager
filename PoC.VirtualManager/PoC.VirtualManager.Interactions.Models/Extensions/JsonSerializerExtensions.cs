using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Interactions.Extensions
{
    public static class JsonSerializerExtensions
    {
        public static bool TryDeserialize(this string json, Type type, out object result)
        {
            try
            {
                json = ExtractJson(json);
                result = JsonSerializer.Deserialize(json, type);
                return true;
            }
            catch (JsonException)
            {
                result = null;
                return false;
            }
        }

        private static string ExtractJson(string input)
        {
            // Find the start index of the JSON content
            int startIndex = input.IndexOf("```json", StringComparison.OrdinalIgnoreCase);
            if (startIndex == -1)
                return input;

            // Find the end index of the JSON content
            int endIndex = input.IndexOf("```", startIndex + 1, StringComparison.OrdinalIgnoreCase);
            if (endIndex == -1)
                return input;

            // Extract the JSON content
            int jsonStartIndex = startIndex + "```json".Length;
            int jsonLength = endIndex - jsonStartIndex;
            string extractedJson = input.Substring(jsonStartIndex, jsonLength);

            return extractedJson.Trim();
        }
    }
}
