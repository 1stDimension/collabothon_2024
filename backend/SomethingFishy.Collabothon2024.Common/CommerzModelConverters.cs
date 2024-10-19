using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using SomethingFishy.Collabothon2024.Common.Models;

namespace SomethingFishy.Collabothon2024.Common;

public sealed class CommerzPhoneTypeConverter : JsonConverter<CommerzPhoneType>
{
    public override CommerzPhoneType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new FormatException($"Expected string, got {reader.TokenType} instead (converting {typeof(CommerzPhoneType)}).");

        var val = reader.GetString();
        return val switch
        {
            "mobile phone" => CommerzPhoneType.Mobile,
            "phone" => CommerzPhoneType.Landline,
            _ => throw new ArgumentOutOfRangeException(nameof(val), "Unrecognized phone type.")
        };
    }

    public override void Write(Utf8JsonWriter writer, CommerzPhoneType value, JsonSerializerOptions options)
    {
        var val = value switch
        {
            CommerzPhoneType.Landline => "phone",
            CommerzPhoneType.Mobile => "mobile phone",
            _ => throw new ArgumentOutOfRangeException(nameof(value), "Unrecognized phone type.")
        };

        writer.WriteStringValue(val);
    }
}

public sealed class CommerzAddressTypeConverter : JsonConverter<CommerzAddressType>
{
    public override CommerzAddressType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new FormatException($"Expected string, got {reader.TokenType} instead (converting {typeof(CommerzAddressType)}).");

        var val = reader.GetString();
        return val switch
        {
            "legal address" => CommerzAddressType.Legal,
            _ => throw new ArgumentOutOfRangeException(nameof(val), "Unrecognized address type.")
        };
    }

    public override void Write(Utf8JsonWriter writer, CommerzAddressType value, JsonSerializerOptions options)
    {
        var val = value switch
        {
            CommerzAddressType.Legal => "legal address",
            _ => throw new ArgumentOutOfRangeException(nameof(value), "Unrecognized address type.")
        };

        writer.WriteStringValue(val);
    }
}

public sealed class CommerzYesNoConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new FormatException($"Expected string, got {reader.TokenType} instead (converting {typeof(bool)}).");

        var val = reader.GetString();
        return val switch
        {
            "y" or "true" => true,
            "n" or "false" => false,
            _ => throw new ArgumentOutOfRangeException(nameof(val), "Unrecognized yes/no value."),
        };
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        var val = value switch
        {
            true => "true",
            false => "false",
        };

        writer.WriteStringValue(val);
    }
}
