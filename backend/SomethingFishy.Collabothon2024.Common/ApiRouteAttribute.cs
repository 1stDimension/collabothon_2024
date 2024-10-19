using System;

namespace SomethingFishy.Collabothon2024.Common;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class ApiRouteAttribute : Attribute
{
    public ApiMethod Method { get; }

    public string Endpoint { get; }

    public ApiRouteAttribute(ApiMethod method, string endpoint)
    {
        this.Endpoint = endpoint;
    }
}

public enum ApiMethod
{
    GET,
    POST,
    PUT,
    DELETE,
    PATCH,
}
