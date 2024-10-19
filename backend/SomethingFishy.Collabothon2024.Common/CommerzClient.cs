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
using SomethingFishy.Collabothon2024.Common.Models;

namespace SomethingFishy.Collabothon2024.Common;

internal sealed class CommerzClient : ICommerzClient, ICommerzAccountsForeignUnitsClient, ICommerzCorporatePaymentsClient, ICommerzInstantNotificationsClient, ICommerzCustomersClient, ICommerzSecuritiesClient
{
    private static Uri UriAccountForeignUnits { get; } = new("https://api-sandbox.commerzbank.com/accounts-api/21/v1");
    private static Uri UriCorporatePayments { get; } = new("https://api-sandbox.commerzbank.com/corporate-payments-api/1/v1/bulk-payments");
    private static Uri UriInstantPaymentNotifications { get; } = new("https://api-sandbox.commerzbank.com/payments-api/12/v1");
    private static Uri UriCustomers { get; } = new("https://api-sandbox.commerzbank.com/customers-api/v2");
    private static Uri UriSecurities { get; } = new("https://api-sandbox.commerzbank.com/securities-api/v4");
    private static JsonSerializerOptions JsonOptions { get; } = JsonSerializerOptions.Default.WithCommerzConverters();

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
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriAccountForeignUnits).WithAccessToken(this._authToken);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzAccountList>(JsonOptions, cancellationToken);
    }

    [ApiRoute(ApiMethod.GET, $"/accounts/:{nameof(accountId)}")]
    async Task<CommerzAccount> ICommerzAccountsForeignUnitsClient.GetAccountAsync(string accountId, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriAccountForeignUnits, new { accountId }).WithAccessToken(this._authToken);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzAccount>(JsonOptions, cancellationToken);
    }

    [ApiRoute(ApiMethod.GET, $"/accounts/:{nameof(accountId)}/balances")]
    async Task<CommerzAccountBalances> ICommerzAccountsForeignUnitsClient.GetAccountBalanceListAsync(string accountId, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriAccountForeignUnits, new { accountId }).WithAccessToken(this._authToken);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzAccountBalances>(JsonOptions, cancellationToken);
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
}
