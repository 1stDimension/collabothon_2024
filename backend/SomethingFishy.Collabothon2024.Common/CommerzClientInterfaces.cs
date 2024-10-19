using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using SomethingFishy.Collabothon2024.Common.Models;

namespace SomethingFishy.Collabothon2024.Common;

public interface ICommerzClient
{
    public string AuthorizationToken { set; }
}

public interface ICommerzAccountsForeignUnitsClient : ICommerzClient
{
    Task<CommerzAccountList> GetAccountListAsync(CancellationToken cancellationToken = default);
    Task<CommerzAccountBalances> GetAccountAsync(string accountId, CancellationToken cancellationToken = default);
    Task<CommerzAccount> GetAccountBalanceListAsync(string accountId, CancellationToken cancellationToken = default);
}

public interface ICommerzCorporatePaymentsClient : ICommerzClient
{
    Task<IEnumerable<CommerzCcscMessage>> GetMessagesAsync(CancellationToken cancellationToken = default);
    Task CreateMessageAsync(Stream data, CancellationToken cancellationToken = default);
    Task GetMessageAsync(string messageId, Stream destination, CancellationToken cancellationToken = default);
    Task SetTransferStatusAsync(string messageId, CommerzCcscMessageStatus status, CancellationToken cancellationToken = default);
}

public interface ICommerzInstantNotificationsClient : ICommerzClient
{
    Task<CommerzSubscriptionResponse> CreateSubscriptionAsync(CommerzSubscriptionRequest subscription, CancellationToken cancellationToken = default);
    Task<CommerzSubscription> GetSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default);
    Task TerminateSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default);
    Task<CommerzSubscriptionStatusResponse> GetSubscriptionStatusAsync(string subscriptionId, CancellationToken cancellationToken = default);
    Task<CommerzSubscriptionEntryResponse> CreateSubscriptionEntryAsync(string subscriptionId, CommerzSubscriptionEntryRequest subscriptionEntry, CancellationToken cancellationToken = default);
    Task<CommerzSubscriptionEntry> GetSubscriptionEntryAsync(string subscriptionId, string subscriptionEntryId, CancellationToken cancellationToken = default);
    Task TerminateSubscriptionEntryAsync(string subscriptionId, string subscriptionEntryId, CancellationToken cancellationToken = default);
    Task<CommerzSubscriptionEntryStatusResponse> GetSubscriptionEntryStatusAsync(string subscriptionId, string subscriptionEntryId, CancellationToken cancellationToken = default);
    Task SupplyNotificationHostCertificateAsync(string hostname, X509Certificate2 certificate, CancellationToken cancellationToken = default);
}

public interface ICommerzCustomersClient : ICommerzClient
{
    Task<CommerzCustomer> GetCurrentCustomerAsync(CancellationToken cancellationToken = default);
}

public interface ICommerzSecuritiesClient : ICommerzClient
{
    Task<CommerzAccountsResponse> GetSecuritiesAccountAsync(CancellationToken cancellationToken = default);
    Task<CommerzPortfolioOverviewResponse> GetSecuritiesPortfolioAsync(string accountId, DateOnly? effectiveDate = default, CancellationToken cancellationToken = default);
    Task<CommerzTransactionsResponse> GetTransactionsAsync(string accountId, CommerzSecurityTransactionType? type = default, DateOnly? fromTradingDate = default, DateOnly? toTradingDate = default, int limit = 25, CancellationToken cancellationToken = default);
}

public interface ICommerzOauthClient
{
    Task<CommerzStampedCredentials> GetClientCredentialsTokenAsync(string clientId, string clientSecret, CancellationToken cancellationToken = default);
    Task<CommerzStampedCredentials> RefreshTokenAsync(string clientId, string clientSecret, string refreshToken, CancellationToken cancellationToken = default);
    Task<CommerzStampedCredentials> GetUserTokenAsync(string clientId, string clientSecret, string authorizationCode, Uri appUri, CancellationToken cancellationToken = default);
    Task<Uri> GetAuthorizationRedirectAsync(string clientId, Uri appUri, CancellationToken cancellationToken = default);
}
