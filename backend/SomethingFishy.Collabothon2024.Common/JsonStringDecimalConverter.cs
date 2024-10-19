using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SomethingFishy.Collabothon2024.Common;

public sealed class JsonStringDecimalConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String && reader.TokenType != JsonTokenType.Number)
            throw new JsonException($"Invalid data type encountered when reading decimal: {reader.TokenType}.");

        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetDecimal();

        var strval = reader.GetString();
        var val = decimal.Parse(strval, CultureInfo.InvariantCulture);
        return val;
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        var strval = value.ToString(CultureInfo.InvariantCulture);
        writer.WriteStringValue(strval);
    }
}
