using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SomethingFishy.Collabothon2024.Common.Models;

namespace SomethingFishy.Collabothon2024.Common;

internal sealed class CommerzClient 
    : ICommerzClient, 
    ICommerzAccountsForeignUnitsClient, 
    ICommerzCorporatePaymentsClient, 
    ICommerzInstantNotificationsClient, 
    ICommerzCustomersClient, 
    ICommerzSecuritiesClient, 
    ICommerzOauthClient
{
    private static Uri UriAccountForeignUnits { get; } = new("https://api-sandbox.commerzbank.com/accounts-api/21/v1");
    private static Uri UriCorporatePayments { get; } = new("https://api-sandbox.commerzbank.com/corporate-payments-api/1/v1/bulk-payments");
    private static Uri UriInstantPaymentNotifications { get; } = new("https://api-sandbox.commerzbank.com/payments-api/12/v1");
    private static Uri UriCustomers { get; } = new("https://api-sandbox.commerzbank.com/customers-api/v2");
    private static Uri UriSecurities { get; } = new("https://api-sandbox.commerzbank.com/securities-api/v4");
    private static Uri UriOauthClient { get; } = new("https://api-sandbox.commerzbank.com/auth/realms/sandbox/protocol/openid-connect/token");
    private static Uri UriOauthUser { get; } = new("https://api-sandbox.commerzbank.com/auth/realms/sandbox/protocol/openid-connect/auth");
    private static JsonSerializerOptions JsonOptions { get; } = JsonSerializerOptions.Default.WithCommerzConverters();
    private static JsonSerializerOptions JsonOauthOptions { get; } = JsonSerializerOptions.Default.ConfigureCommerzOauth();

    string ICommerzClient.AuthorizationToken { set => this._authToken = value; }

    private string _authToken = null;

    private readonly HttpClient _http;

    public CommerzClient(HttpClient http)
    {
        this._http = http;
    }

    // accounts foreign units
    [ApiRoute(ApiMethod.GET, "/accounts")]
    async Task<CommerzAccountList> ICommerzAccountsForeignUnitsClient.GetAccountListAsync(CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriAccountForeignUnits, new { }, new { customerId = "_" }).WithAccessToken(this._authToken);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzAccountList>(JsonOptions, cancellationToken);
    }

    [ApiRoute(ApiMethod.GET, $"/accounts/:{nameof(accountId)}")]
    async Task<CommerzAccountBalances> ICommerzAccountsForeignUnitsClient.GetAccountAsync(string accountId, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriAccountForeignUnits, new { accountId }).WithAccessToken(this._authToken);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzAccountBalances>(JsonOptions, cancellationToken);
    }

    [ApiRoute(ApiMethod.GET, $"/accounts/:{nameof(accountId)}/balances")]
    async Task<CommerzAccount> ICommerzAccountsForeignUnitsClient.GetAccountBalanceListAsync(string accountId, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriAccountForeignUnits, new { accountId }).WithAccessToken(this._authToken);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzAccount>(JsonOptions, cancellationToken);
    }

    // corporate payments
    [ApiRoute(ApiMethod.GET, "/messages")]
    async Task<IEnumerable<CommerzCcscMessage>> ICommerzCorporatePaymentsClient.GetMessagesAsync(CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriCorporatePayments).WithAccessToken(this._authToken);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<IEnumerable<CommerzCcscMessage>>(JsonOptions, cancellationToken);
    }

    [ApiRoute(ApiMethod.POST, "/messages")]
    async Task ICommerzCorporatePaymentsClient.CreateMessageAsync(Stream data, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriCorporatePayments).WithAccessToken(this._authToken);
        req.Content = new StreamContent(data);
        req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    [ApiRoute(ApiMethod.GET, $"/messages/:{nameof(messageId)}")]
    async Task ICommerzCorporatePaymentsClient.GetMessageAsync(string messageId, Stream destination, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriCorporatePayments, new { messageId }).WithAccessToken(this._authToken);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        await res.Content.CopyToAsync(destination, cancellationToken);
    }

    [ApiRoute(ApiMethod.PUT, $"/messages/:{nameof(messageId)}")]
    async Task ICommerzCorporatePaymentsClient.SetTransferStatusAsync(string messageId, CommerzCcscMessageStatus status, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriCorporatePayments, new { messageId }).WithAccessToken(this._authToken);
        req.Content = JsonContent.Create(status, null, JsonOptions);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    // instant notifications
    [ApiRoute(ApiMethod.POST, "/subscriptions/instant-payment-notifications")]
    async Task<CommerzSubscriptionResponse> ICommerzInstantNotificationsClient.CreateSubscriptionAsync(CommerzSubscriptionRequest subscription, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriInstantPaymentNotifications).WithAccessToken(this._authToken);
        req.Content = JsonContent.Create(subscription, null, JsonOptions);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzSubscriptionResponse>(JsonOptions, cancellationToken);
    }

    [ApiRoute(ApiMethod.GET, $"/subscriptions/instant-payment-notifications/:{nameof(subscriptionId)}")]
    async Task<CommerzSubscription> ICommerzInstantNotificationsClient.GetSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriInstantPaymentNotifications, new { subscriptionId }).WithAccessToken(this._authToken);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzSubscription>(JsonOptions, cancellationToken);
    }

    [ApiRoute(ApiMethod.DELETE, $"/subscriptions/instant-payment-notifications/:{nameof(subscriptionId)}")]
    async Task ICommerzInstantNotificationsClient.TerminateSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriInstantPaymentNotifications, new { subscriptionId }).WithAccessToken(this._authToken);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    [ApiRoute(ApiMethod.GET, $"/subscriptions/instant-payment-notifications/:{nameof(subscriptionId)}/status")]
    async Task<CommerzSubscriptionStatusResponse> ICommerzInstantNotificationsClient.GetSubscriptionStatusAsync(string subscriptionId, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriInstantPaymentNotifications, new { subscriptionId }).WithAccessToken(this._authToken);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzSubscriptionStatusResponse>(JsonOptions, cancellationToken);
    }

    [ApiRoute(ApiMethod.GET, $"/subscriptions/instant-payment-notifications/:{nameof(subscriptionId)}/subscription-entries")]
    async Task<CommerzSubscriptionEntryResponse> ICommerzInstantNotificationsClient.CreateSubscriptionEntryAsync(string subscriptionId, CommerzSubscriptionEntryRequest subscriptionEntry, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriInstantPaymentNotifications, new { subscriptionId }).WithAccessToken(this._authToken);
        req.Content = JsonContent.Create(subscriptionEntry, null, JsonOptions);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzSubscriptionEntryResponse>(JsonOptions, cancellationToken);
    }

    [ApiRoute(ApiMethod.GET, $"/subscriptions/instant-payment-notifications/:{nameof(subscriptionId)}/subscription-entries/:{nameof(subscriptionEntryId)}")]
    async Task<CommerzSubscriptionEntry> ICommerzInstantNotificationsClient.GetSubscriptionEntryAsync(string subscriptionId, string subscriptionEntryId, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriInstantPaymentNotifications, new { subscriptionId, subscriptionEntryId }).WithAccessToken(this._authToken);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzSubscriptionEntry>(JsonOptions, cancellationToken);
    }

    [ApiRoute(ApiMethod.DELETE, $"/subscriptions/instant-payment-notifications/:{nameof(subscriptionId)}/subscription-entries/:{nameof(subscriptionEntryId)}")]
    async Task ICommerzInstantNotificationsClient.TerminateSubscriptionEntryAsync(string subscriptionId, string subscriptionEntryId, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriInstantPaymentNotifications, new { subscriptionId, subscriptionEntryId }).WithAccessToken(this._authToken);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    [ApiRoute(ApiMethod.GET, $"/subscriptions/instant-payment-notifications/:{nameof(subscriptionId)}/subscription-entries/:{nameof(subscriptionEntryId)}/status")]
    async Task<CommerzSubscriptionEntryStatusResponse> ICommerzInstantNotificationsClient.GetSubscriptionEntryStatusAsync(string subscriptionId, string subscriptionEntryId, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriInstantPaymentNotifications, new { subscriptionId, subscriptionEntryId }).WithAccessToken(this._authToken);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzSubscriptionEntryStatusResponse>(JsonOptions, cancellationToken);
    }

    [ApiRoute(ApiMethod.POST, "/subscriptions/instant-payment-notifications/certificates")]
    async Task ICommerzInstantNotificationsClient.SupplyNotificationHostCertificateAsync(string hostname, X509Certificate2 certificate, CancellationToken cancellationToken)
    {
        var pem = certificate.ExportCertificatePem();
        var data = new CommerzPostCertificateRequest
        {
            Hostname = hostname,
            Certificate = pem,
        };

        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriInstantPaymentNotifications).WithAccessToken(this._authToken);
        req.Content = JsonContent.Create(data, null, JsonOptions);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    // customers
    [ApiRoute(ApiMethod.GET, "/customers")]
    async Task<CommerzCustomer> ICommerzCustomersClient.GetCurrentCustomerAsync(CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriCustomers).WithAccessToken(this._authToken);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzCustomer>(JsonOptions, cancellationToken);
    }

    // securities
    [ApiRoute(ApiMethod.GET, "/accounts")]
    async Task<CommerzAccountsResponse> ICommerzSecuritiesClient.GetSecuritiesAccountAsync(CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriSecurities).WithAccessToken(this._authToken);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzAccountsResponse>(JsonOptions, cancellationToken);
    }

    [ApiRoute(ApiMethod.GET, $"/accounts/:{nameof(accountId)}/portfolio")]
    async Task<CommerzPortfolioOverviewResponse> ICommerzSecuritiesClient.GetSecuritiesPortfolioAsync(string accountId, DateOnly? effectiveDate, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriSecurities, new { accountId }, new { effectiveDate }).WithAccessToken(this._authToken);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzPortfolioOverviewResponse>(JsonOptions, cancellationToken);
    }

    [ApiRoute(ApiMethod.GET, $"/accounts/:{nameof(accountId)}/transactions")]
    async Task<CommerzTransactionsResponse> ICommerzSecuritiesClient.GetTransactionsAsync(string accountId, CommerzSecurityTransactionType? type, DateOnly? fromTradingDate, DateOnly? toTradingDate, int limit, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriSecurities, new { accountId }, new { type, fromTradingDate, toTradingDate, limit }).WithAccessToken(this._authToken);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzTransactionsResponse>(JsonOptions, cancellationToken);
    }

    // oauth
    async Task<CommerzStampedCredentials> ICommerzOauthClient.GetClientCredentialsTokenAsync(string clientId, string clientSecret, CancellationToken cancellationToken)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, UriOauthClient)
        {
            Content = new FormUrlEncodedContent([
                new("grant_type", "client_credentials"),
                new("client_id", clientId),
                new("client_secret", clientSecret),
            ])
        };
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        var creds = await res.Content.ReadFromJsonAsync<CommerzCredentials>(JsonOauthOptions, cancellationToken);
        var date = res.Headers.Date;
        return new() { ServerTime = date ?? DateTimeOffset.UtcNow, Credentials = creds };
    }

    async Task<CommerzStampedCredentials> ICommerzOauthClient.RefreshTokenAsync(string clientId, string clientSecret, string refreshToken, CancellationToken cancellationToken)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, UriOauthClient)
        {
            Content = new FormUrlEncodedContent([
                new("grant_type", "refresh_token"),
                new("refresh_token", refreshToken),
                new("client_id", clientId),
                new("client_secret", clientSecret),
            ])
        };
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        var creds = await res.Content.ReadFromJsonAsync<CommerzCredentials>(JsonOauthOptions, cancellationToken);
        var date = res.Headers.Date;
        return new() { ServerTime = date ?? DateTimeOffset.UtcNow, Credentials = creds };
    }

    async Task<CommerzStampedCredentials> ICommerzOauthClient.GetUserTokenAsync(string clientId, string clientSecret, string authorizationCode, Uri appUri, CancellationToken cancellationToken)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, UriOauthClient)
        {
            Content = new FormUrlEncodedContent([
                new("grant_type", "authorization_code"),
                new("client_id", clientId),
                new("client_secret", clientSecret),
                new("code", authorizationCode),
                new("redirect_uri", appUri.ToString())
            ])
        };
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        var creds = await res.Content.ReadFromJsonAsync<CommerzCredentials>(JsonOauthOptions, cancellationToken);
        var date = res.Headers.Date;
        return new() { ServerTime = date ?? DateTimeOffset.UtcNow, Credentials = creds };
    }

    Task<Uri> ICommerzOauthClient.GetAuthorizationRedirectAsync(string clientId, Uri appUri, CancellationToken cancellationToken)
    {
        var query = QueryString.Create([
            new KeyValuePair<string, string>("response_type", "code"),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("redirect_uri", appUri.ToString()),
        ]);

        var ub = new UriBuilder(UriOauthUser)
        {
            Query = query.Value
        };

        return Task.FromResult(ub.Uri);
    }
}
