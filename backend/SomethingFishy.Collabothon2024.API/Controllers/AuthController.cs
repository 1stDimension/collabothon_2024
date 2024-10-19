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
        var ub = new UriBuilder(this.Request.GetEncodedUrl())
        {
            Path = path,
            Query = ""
        };

        var uri = await this._oauth.GetAuthorizationRedirectAsync(this._config.ClientId, ub.Uri, cancellationToken);
        return this.Redirect(uri.ToString());
    }

    [HttpGet, Route("user/complete", Name = nameof(CompleteUserAuthenticationAsync))]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> CompleteUserAuthenticationAsync([FromQuery] string code, CancellationToken cancellationToken = default)
    {
        var path = this.Url.RouteUrl(routeName: nameof(this.CompleteUserAuthenticationAsync));
        var ub = new UriBuilder(this.Request.GetEncodedUrl())
        {
            Path = path,
            Query = ""
        };

        var creds = await this._oauth.GetUserTokenAsync(this._config.ClientId, this._config.ClientSecret, code, ub.Uri, cancellationToken);
        var token = this._tokenHandler.Issue(creds);

        this.Response.Headers.Append(AuthenticationTokenHandler.HeaderUpdateToken, new(token));
        return this.Redirect("/");
    }
}
