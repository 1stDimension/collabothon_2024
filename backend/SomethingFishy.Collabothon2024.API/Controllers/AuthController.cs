using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SomethingFishy.Collabothon2024.API.Data;
using SomethingFishy.Collabothon2024.API.Services;
using SomethingFishy.Collabothon2024.Common;

namespace SomethingFishy.Collabothon2024.API.Controllers;

[ApiController, AllowAnonymous]
[Route("/api/v1/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly ICommerzOauthClient _oauth;
    private readonly AuthenticationTokenHandler _tokenHandler;
    private readonly ApplicationConfiguration _config;

    public AuthController(
        ICommerzOauthClient oauth,
        AuthenticationTokenHandler tokenHandler,
        IOptions<ApplicationConfiguration> options)
    {
        this._oauth = oauth;
        this._tokenHandler = tokenHandler;
        this._config = options.Value;
    }

    [HttpGet, Route("user")]
    public async Task<IActionResult> AuthenticateUserAsync(CancellationToken cancellationToken = default)
    {
        var path = this.Url.RouteUrl(routeName: nameof(this.CompleteUserAuthenticationAsync));
        var ub = new UriBuilder
        {
            Scheme = this.HttpContext.Request.Scheme,
            Host = this.HttpContext.Request.Host.Host,
            Path = path,
            Query = ""
        };

        var port = this.HttpContext.Request.Host.Port;
        if (port is not null)
            ub.Port = port.Value;

        var uri = await this._oauth.GetAuthorizationRedirectAsync(this._config.ClientId, ub.Uri, cancellationToken);
        return this.Redirect(uri.ToString());
    }

    [HttpGet, Route("user/complete", Name = nameof(CompleteUserAuthenticationAsync))]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> CompleteUserAuthenticationAsync([FromQuery] string code, CancellationToken cancellationToken = default)
    {
        var path = this.Url.RouteUrl(routeName: nameof(this.CompleteUserAuthenticationAsync));
        var ub = new UriBuilder
        {
            Scheme = this.HttpContext.Request.Scheme,
            Host = this.HttpContext.Request.Host.Host,
            Path = path,
            Query = ""
        };

        var port = this.HttpContext.Request.Host.Port;
        if (port is not null)
            ub.Port = port.Value;

        var creds = await this._oauth.GetUserTokenAsync(this._config.ClientId, this._config.ClientSecret, code, ub.Uri, cancellationToken);
        var token = this._tokenHandler.Issue(creds);

        var tokenFragment = QueryString.Create("@token", token);
        ub = new UriBuilder(this.Request.GetEncodedUrl())
        {
            Path = "/",
            Query = "",
            Fragment = tokenFragment.Value[1..]
        };
        return this.Redirect(ub.Uri.ToString());
    }
}
