using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SomethingFishy.Collabothon2024.API.Data;
using SomethingFishy.Collabothon2024.API.Services;
using SomethingFishy.Collabothon2024.Common;

namespace SomethingFishy.Collabothon2024.API;

public class Program
{
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

        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddCommerzClient();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
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

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();

        //using (var scope = app.Services.CreateScope())
        //{
        //    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        //    db.Database.Migrate();
        //}

        app.Run();
    }
}
