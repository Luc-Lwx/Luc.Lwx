using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Luc.Lwx.Util;

public class LwxJsonConverter_DateTimeField : JsonConverter<DateTime>
{
    private const string Format = "yyyy-MM-dd'T'HH:mm:ss.fffK";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dhString = reader.GetString();
        if( dhString == null )
        {
            return DateTime.MinValue;
        }
        else 
        {
            if (DateTime.TryParseExact(dhString, Format, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dateTimeWithFractionalSeconds))
            {
                return dateTimeWithFractionalSeconds;
            }
            return DateTime.Parse(dhString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }
}