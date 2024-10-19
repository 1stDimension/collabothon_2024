using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SomethingFishy.Collabothon2024.API.Data;
using SomethingFishy.Collabothon2024.API.Services;
using SomethingFishy.Collabothon2024.Common;

namespace SomethingFishy.Collabothon2024.API;

public class Program
{
    private const string _corsPolicyName = "allowEverythingPolicy";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var config = new ConfigurationBuilder()
            .AddConfiguration(builder.Configuration)
            .AddJsonFile("config.json", optional: true)
            .AddEnvironmentVariables("STHFISHY:")
            .AddCommandLine(args)
            .Build();

        // Add services to the container.
        builder.Services.AddOptions<ApplicationConfiguration>()
            .Bind(config)
            .ValidateDataAnnotations();

        builder.Services.AddOptions<JwtConfiguration>()
            .Bind(config.GetSection("JWT"))
            .ValidateDataAnnotations();

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.All;
            options.AllowedHosts.Clear();
            options.KnownNetworks.Clear();
            options.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.Any, 0));
            options.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.IPv6Any, 0));
        });

        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddCommerzClient();

        // Add services to the container.
        builder.Services.AddCors(opts =>
            opts.AddPolicy(_corsPolicyName, policy =>
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()));

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                options.JsonSerializerOptions.Converters.Add(new JsonStringDecimalConverter());
                options.JsonSerializerOptions.Converters.Add(new JsonDateOnlyConverter());
            });

        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtConfig = new JwtConfiguration();
                config.GetSection("JWT").Bind(jwtConfig);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtConfig.Key),
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddHostedService<ClientCredentialsService>();
        builder.Services.AddScoped<AuthenticationTokenHandler>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(config =>
        {
            config.AddSecurityDefinition("JWT Bearer", new()
            {
                Description = "JWT Bearer authentication scheme using Authorization header with a value of 'Bearer [token]'.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
            });

            config.AddSecurityRequirement(new()
            {
                [
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "JWT Bearer",
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    }
                ] = []
            });
        });

        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        app.UseForwardedHeaders();
        app.UseCors(_corsPolicyName);

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseAuthentication();
        app.UseAuthorization();

        app.Use(HandleTokenRotationsAsync);

        app.MapControllers();

        //using (var scope = app.Services.CreateScope())
        //{
        //    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        //    db.Database.Migrate();
        //}

        app.Run();
    }

    private static async Task HandleTokenRotationsAsync(HttpContext ctx, RequestDelegate next)
    {
        if (ctx.TryGetCommerzCredentials(out var credentials) && credentials.TokenExpiresAt.AddSeconds(-15) <= DateTimeOffset.UtcNow)
        {
            var services = ctx.RequestServices;
            var oauth = services.GetRequiredService<ICommerzOauthClient>();
            var tokens = services.GetRequiredService<AuthenticationTokenHandler>();
            var options = services.GetRequiredService<IOptions<ApplicationConfiguration>>();
            var config = options.Value;
            try
            {
                var creds = await oauth.RefreshTokenAsync(config.ClientId, config.ClientSecret, credentials.RefreshToken, ctx.RequestAborted);
                var token = tokens.Issue(creds);
                ctx.Request.Headers.Authorization = new(token);
                ctx.Response.Headers.Append(AuthenticationTokenHandler.HeaderUpdateToken, token);
                ctx.User = ctx.User.UpdateWith(creds);
            }
            catch
            {
                ctx.Request.Headers.Remove("Authorization");
                ctx.User = new ClaimsPrincipal();
                ctx.Response.Headers.Append(AuthenticationTokenHandler.HeaderUpdateToken, "");
            }
        }

        await next(ctx);
    }
}
