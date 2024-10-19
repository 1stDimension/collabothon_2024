using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SomethingFishy.Collabothon2024.API.Data;
using SomethingFishy.Collabothon2024.Common;
using SomethingFishy.Collabothon2024.Common.Models;

namespace SomethingFishy.Collabothon2024.API.Services;

public sealed class ClientCredentialsService : IHostedService
{
    public string Token
        => this._credentials.Credentials.AccessToken;

    private readonly ICommerzOauthClient _oauth;
    private readonly ApplicationConfiguration _config;
    private readonly ILogger<ClientCredentialsService> _logger;
    private readonly CancellationTokenSource _tokenSource;

    private CommerzStampedCredentials _credentials;

    public ClientCredentialsService(
        ICommerzOauthClient oauth,
        IOptions<ApplicationConfiguration> options,
        ILogger<ClientCredentialsService> logger)
    {
        this._oauth = oauth;
        this._config = options.Value;
        this._logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Initializing application credentials service");
        var creds = await this._oauth.GetClientCredentialsTokenAsync(this._config.ClientId, this._config.ClientSecret, cancellationToken);
        this._logger.LogInformation("Application credentials service initialized with tokens");
        this._credentials = creds;
        _ = this.RefreshLoop();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        this._tokenSource.Cancel();
        return Task.CompletedTask;
    }

    private async Task RefreshLoop()
    {
        var token = this._tokenSource.Token;
        while (!token.IsCancellationRequested)
        {
            var creds = this._credentials;
            var refreshDeadline = creds.ServerTime.AddSeconds(creds.Credentials.ExpiresIn - 15);
            var delay = refreshDeadline - DateTimeOffset.UtcNow;
            await Task.Delay(delay, token);
            this._logger.LogInformation("Application token expired, refreshing");
            this._credentials = await this._oauth.RefreshTokenAsync(this._config.ClientId, this._config.ClientSecret, creds.Credentials.RefreshToken, token);
        }
    }
}
