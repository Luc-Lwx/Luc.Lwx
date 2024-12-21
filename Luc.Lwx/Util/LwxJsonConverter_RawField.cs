using System.Text.Json;
using System.Text.Json.Serialization;

namespace Luc.Lwx.Util;

public class LwxJsonConverter_RawField : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      using var jsonDoc = JsonDocument.ParseValue(ref reader);
      return jsonDoc.RootElement.GetRawText();
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
      if (value == null)
      {
        writer.WriteNullValue();
      }
      else if (value.Trim().StartsWith('{') && value.Trim().EndsWith('}'))
      {
        writer.WriteRawValue(value);
      }
      else
      {
        throw new JsonException("Invalid JSON object format.");
      }
    }
}