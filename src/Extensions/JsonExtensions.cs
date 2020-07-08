using System.Text.Json;

namespace Vysn.Commons.Extensions {
    /// <summary>
    /// 
    /// </summary>
    public static class JsonExtensions {
        internal static bool TryRead(this ref Utf8JsonReader reader, string content) {
            return reader.TokenType == JsonTokenType.PropertyName && reader.ValueTextEquals(content) && reader.Read();
        }
    }
}