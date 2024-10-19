using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
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
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriAccountForeignUnits);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzAccountList>(cancellationToken);
    }

    [ApiRoute(ApiMethod.GET, $"/accounts/:{nameof(accountId)}")]
    async Task<CommerzAccount> ICommerzAccountsForeignUnitsClient.GetAccountAsync(string accountId, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriAccountForeignUnits, new { accountId });
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzAccount>(cancellationToken);
    }

    [ApiRoute(ApiMethod.GET, $"/accounts/:{nameof(accountId)}/balances")]
    async Task<CommerzAccountBalances> ICommerzAccountsForeignUnitsClient.GetAccountBalanceListAsync(string accountId, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriAccountForeignUnits, new { accountId });
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzAccountBalances>(cancellationToken);
    }

    // corporate payments
    [ApiRoute(ApiMethod.GET, "/messages")]
    async Task<IEnumerable<CommerzCcscMessage>> ICommerzCorporatePaymentsClient.GetMessagesAsync(CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriCorporatePayments);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<IEnumerable<CommerzCcscMessage>>(cancellationToken);
    }

    [ApiRoute(ApiMethod.POST, "/messages")]
    async Task ICommerzCorporatePaymentsClient.CreateMessageAsync(Stream data, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriCorporatePayments);
        req.Content = new StreamContent(data);
        req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    [ApiRoute(ApiMethod.GET, $"/messages/:{nameof(messageId)}")]
    async Task ICommerzCorporatePaymentsClient.GetMessageAsync(string messageId, Stream destination, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriCorporatePayments, new { messageId });
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        await res.Content.CopyToAsync(destination);
    }

    [ApiRoute(ApiMethod.PUT, $"/messages/:{nameof(messageId)}")]
    async Task ICommerzCorporatePaymentsClient.SetTransferStatusAsync(string messageId, CommerzCcscMessageStatus status, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriCorporatePayments, new { messageId });
        req.Content = JsonContent.Create(status);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    // instant notifications
    [ApiRoute(ApiMethod.POST, "/subscriptions/instant-payment-notifications")]
    Task<CommerzSubscriptionResponse> ICommerzInstantNotificationsClient.CreateSubscriptionAsync(CommerzSubscriptionRequest subscription, CancellationToken cancellationToken) => throw new NotImplementedException();

    [ApiRoute(ApiMethod.GET, $"/subscriptions/instant-payment-notifications/:{nameof(subscriptionId)}")]
    Task<CommerzSubscription> ICommerzInstantNotificationsClient.GetSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken) => throw new NotImplementedException();

    [ApiRoute(ApiMethod.DELETE, $"/subscriptions/instant-payment-notifications/:{nameof(subscriptionId)}")]
    Task ICommerzInstantNotificationsClient.TerminateSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken) => throw new NotImplementedException();

    [ApiRoute(ApiMethod.GET, $"/subscriptions/instant-payment-notifications/:{nameof(subscriptionId)}/status")]
    Task<CommerzSubscriptionStatusResponse> ICommerzInstantNotificationsClient.GetSubscriptionStatusAsync(string subscriptionId, CancellationToken cancellationToken) => throw new NotImplementedException();

    [ApiRoute(ApiMethod.GET, $"/subscriptions/instant-payment-notifications/:{nameof(subscriptionId)}/subscription-entries")]
    Task<CommerzSubscriptionEntryResponse> ICommerzInstantNotificationsClient.CreateSubscriptionEntryAsync(string subscriptionId, CommerzSubscriptionEntryRequest subscriptionEntry, CancellationToken cancellationToken) => throw new NotImplementedException();

    [ApiRoute(ApiMethod.GET, $"/subscriptions/instant-payment-notifications/:{nameof(subscriptionId)}/subscription-entries/:{nameof(subscriptionEntryId)}")]
    Task<CommerzSubscriptionEntry> ICommerzInstantNotificationsClient.GetSubscriptionEntryAsync(string subscriptionId, string subscriptionEntryId, CancellationToken cancellationToken) => throw new NotImplementedException();

    [ApiRoute(ApiMethod.DELETE, $"/subscriptions/instant-payment-notifications/:{nameof(subscriptionId)}/subscription-entries/:{nameof(subscriptionEntryId)}")]
    Task ICommerzInstantNotificationsClient.TerminateSubscriptionEntryAsync(string subscriptionId, string subscriptionEntryId, CancellationToken cancellationToken) => throw new NotImplementedException();

    [ApiRoute(ApiMethod.GET, $"/subscriptions/instant-payment-notifications/:{nameof(subscriptionId)}/subscription-entries/:{nameof(subscriptionEntryId)}/status")]
    Task<CommerzSubscriptionEntryStatusResponse> ICommerzInstantNotificationsClient.GetSubscriptionEntryStatusAsync(string subscriptionId, string subscriptionEntryId, CancellationToken cancellationToken) => throw new NotImplementedException();

    [ApiRoute(ApiMethod.POST, "/subscriptions/instant-payment-notifications/certificates")]
    async Task ICommerzInstantNotificationsClient.SupplyNotificationHostCertificateAsync(string hostname, X509Certificate2 certificate, CancellationToken cancellationToken)
    {
        var pem = certificate.ExportCertificatePem();
        var data = new CommerzPostCertificateRequest
        {
            Hostname = hostname,
            Certificate = pem,
        };

        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriInstantPaymentNotifications);
        req.Content = JsonContent.Create(data);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
    }

    // customers
    [ApiRoute(ApiMethod.GET, "/customers")]
    async Task<CommerzCustomer> ICommerzCustomersClient.GetCurrentCustomerAsync(CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriCustomers);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzCustomer>(cancellationToken);
    }

    // securities
    [ApiRoute(ApiMethod.GET, "/accounts")]
    async Task<CommerzAccountsResponse> ICommerzSecuritiesClient.GetSecuritiesAccountAsync(CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriSecurities);
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzAccountsResponse>(cancellationToken);
    }

    [ApiRoute(ApiMethod.GET, $"/accounts/:{nameof(accountId)}/portfolio")]
    async Task<CommerzPortfolioOverviewResponse> ICommerzSecuritiesClient.GetSecuritiesPortfolioAsync(string accountId, DateOnly? effectiveDate, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriSecurities, new { accountId }, new { effectiveDate });
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzPortfolioOverviewResponse>(cancellationToken);
    }

    [ApiRoute(ApiMethod.GET, $"/accounts/:{nameof(accountId)}/transactions")]
    async Task<CommerzTransactionsResponse> ICommerzSecuritiesClient.GetTransactionsAsync(string accountId, CommerzSecurityTransactionType? type, DateOnly? fromTradingDate, DateOnly? toTradingDate, int limit, CancellationToken cancellationToken)
    {
        using var req = ApiRequestBuilder<CommerzClient>.FromRequestContext(UriSecurities, new { accountId }, new { type, fromTradingDate, toTradingDate, limit });
        using var res = await this._http.SendAsync(req, cancellationToken);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CommerzTransactionsResponse>(cancellationToken);
    }
}
