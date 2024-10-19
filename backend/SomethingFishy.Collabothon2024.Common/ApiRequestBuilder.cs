using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Emzi0767.Utilities;
using Microsoft.AspNetCore.Http;

namespace SomethingFishy.Collabothon2024.Common;

internal sealed class ApiRequestBuilder<T> where T : class
{
    private static IReadOnlyDictionary<string, ApiRouteInfo> RouteInfos { get; }
    private static object EmptyParamsObject { get; } = new();
    private static IReadOnlyDictionary<string, object> EmptyParamsDict { get; } = new Dictionary<string, object>();

    static ApiRequestBuilder()
    {
        var t = typeof(T);
        var parsedInfos = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(x => new { m = x, a = x.GetCustomAttribute<ApiRouteAttribute>() })
            .Where(x => x.a is not null)
            .Select(x => new { x.m, i = new ApiRouteInfo { Route = x.a, Fills = ApiRouteParser.ParseRoute(x.a).ToArray() } });

        RouteInfos = parsedInfos.ToDictionary(x => x.m.Name.Split('.')[^1], x => x.i);
    }

    public static HttpRequestMessage FromRequestContext(Uri baseUri, [CallerMemberName] string name = default)
        => FromRequestContext(baseUri, EmptyParamsObject, name);

    public static HttpRequestMessage FromRequestContext<TParams>(Uri baseUri, TParams @params, [CallerMemberName] string name = default)
        where TParams : class
        => FromRequestContext(baseUri, @params, EmptyParamsObject, name);

    public static HttpRequestMessage FromRequestContext<TParams, TQuery>(Uri baseUri, TParams @params, TQuery query, [CallerMemberName] string name = default)
        where TParams : class
        where TQuery : class
    {
        if (!RouteInfos.TryGetValue(name, out var routeInfo))
            throw new InvalidOperationException("Invalid request context used.");

        var req = new HttpRequestMessage
        {
            Method = routeInfo.Route.Method switch
            {
                ApiMethod.GET => HttpMethod.Get,
                ApiMethod.POST => HttpMethod.Post,
                ApiMethod.PUT => HttpMethod.Put,
                ApiMethod.PATCH => HttpMethod.Patch,
                ApiMethod.DELETE => HttpMethod.Delete,
                _ => throw new InvalidOperationException("Request context specifies invalid method.")
            }
        };

        var routeParams = object.ReferenceEquals(@params, EmptyParamsObject)
            ? EmptyParamsDict
            : @params.ToDictionary();

        var endpoint = string.Join("", routeInfo.Fills.Select(x => x.Fill(routeParams)));

        var ub = new UriBuilder(baseUri);
        ub.Path += endpoint;
        if (query is not null && !object.ReferenceEquals(query, EmptyParamsObject))
        {
            var queryString = QueryString.Create(query.ToDictionary().Where(x => x.Value is not null).Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToString())));
            ub.Query = queryString.Value;
        }

        req.RequestUri = ub.Uri;

        return req;
    }
}

internal static class ApiRouteParser
{
    private static readonly Regex _routeInfoRegex = new(@":(?<name>[a-z][a-zA-Z0-9]+)(?:\((?<format>.*)\))?(?=/)?", RegexOptions.Compiled);

    public static IEnumerable<IApiRouteFill> ParseRoute(ApiRouteAttribute apiRoute)
    {
        var endpoint = apiRoute.Endpoint;
        var matches = _routeInfoRegex.Matches(endpoint);
        if (!matches.Any())
        {
            yield return new ApiRouteConstantFill(endpoint);
            yield break;
        }

        var pos = 0;
        foreach (Match match in matches)
        {
            var @const = match.Index;
            if (@const > pos)
            {
                yield return new ApiRouteConstantFill(endpoint[pos..@const]);
                pos = @const;
            }

            var name = match.Groups["name"];
            var format = match.Groups["format"];
            yield return new ApiRouteParameterFill(name.Value, format.Success ? format.Value : null);

            pos = match.Index + match.Length;
        }

        if (pos + 1 < endpoint.Length)
            yield return new ApiRouteConstantFill(endpoint[pos..]);
    }
}

internal readonly struct ApiRouteInfo
{
    public ApiRouteAttribute Route { get; init; }
    public IEnumerable<IApiRouteFill> Fills { get; init; }
}

internal interface IApiRouteFill
{
    string Fill(IReadOnlyDictionary<string, object> parameters);
}

internal readonly struct ApiRouteConstantFill : IApiRouteFill
{
    public string Value { get; }

    public ApiRouteConstantFill(string value)
    {
        this.Value = value;
    }

    string IApiRouteFill.Fill(IReadOnlyDictionary<string, object> parameters)
        => this.Value;
}

internal readonly struct ApiRouteParameterFill : IApiRouteFill
{
    public string Parameter { get; }
    public string Format { get; }

    public ApiRouteParameterFill(string parameter, string format)
    {
        this.Parameter = parameter;
        this.Format = format;
    }

    string IApiRouteFill.Fill(IReadOnlyDictionary<string, object> parameters)
        => this.Format is null
        ? parameters[this.Parameter].ToString()
        : string.Format($"{{0:{this.Format}}}", parameters[this.Parameter]);
}
