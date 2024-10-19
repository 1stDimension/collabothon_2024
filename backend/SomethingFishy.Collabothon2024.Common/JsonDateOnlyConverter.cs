using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SomethingFishy.Collabothon2024.Common;

public sealed class JsonDateOnlyConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException($"Invalid data type encountered when reading DateOnly: {reader.TokenType}.");

        var strval = reader.GetString();
        //var val = DateOnly.ParseExact(strval, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        if (!DateOnly.TryParseExact(strval, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var val) && !DateOnly.TryParseExact(strval, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out val))
            throw new JsonException($"Invalid data type encountered when reading DateOnly {strval}.");

        return val;
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        var strval = value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        writer.WriteStringValue(strval);
    }
}
