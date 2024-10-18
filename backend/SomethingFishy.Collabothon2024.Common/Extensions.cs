using System.Text.Json;
using System.Text.Json.Serialization;

namespace SomethingFishy.Collabothon2024.Common;

public static class Extensions
{
    public static JsonSerializerOptions WithCommerzConverters(this JsonSerializerOptions options)
    {
        options.Converters.Add(new CommerzPhoneTypeConverter());
        options.Converters.Add(new CommerzAddressTypeConverter());
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}
