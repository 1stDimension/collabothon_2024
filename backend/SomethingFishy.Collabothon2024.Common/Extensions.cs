using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Reflection;

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

    public static IServiceCollection AddCommerzClient(this IServiceCollection services)
        => services.AddScoped<ICommerzAccountsForeignUnitsClient, CommerzClient>()
            .AddScoped<ICommerzCorporatePaymentsClient, CommerzClient>()
            .AddScoped<ICommerzInstantNotificationsClient, CommerzClient>()
            .AddScoped<ICommerzCustomersClient, CommerzClient>()
            .AddScoped<ICommerzSecuritiesClient, CommerzClient>();

    internal static HttpRequestMessage WithAccessToken(this HttpRequestMessage req, string token)
    {
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return req;
    }
}
