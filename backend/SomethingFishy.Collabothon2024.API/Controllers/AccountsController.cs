using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SomethingFishy.Collabothon2024.API.Data.Models;
using SomethingFishy.Collabothon2024.Common;

namespace SomethingFishy.Collabothon2024.API.Controllers;

[ApiController, Authorize]
[Route("/api/v1/[controller]")]
public sealed class AccountsController : ControllerBase
{
    private readonly ICommerzAccountsForeignUnitsClient _accounts;

    public AccountsController(ICommerzAccountsForeignUnitsClient accounts)
    {
        this._accounts = accounts;
    }

    [HttpGet, Route("")]
    public async Task<IEnumerable<AccountSummaryModel>> GetAccountSummariesAsync(CancellationToken cancellationToken = default)
    {
        if (!this.HttpContext.TryGetCommerzCredentials(out var credentials))
            return null;

        this._accounts.AuthorizationToken = credentials.AuthenticationToken;
        var accounts = await this._accounts.GetAccountListAsync(cancellationToken);
        return accounts.Accounts.Select(x => new AccountSummaryModel
        {
            AccountId = x.AccountId,
            AccountNumberDisplay = x.AccountNumberDisplay,
            Currency = x.Currency,
        });
    }

    [HttpGet, Route("{id}")]
    public async Task<AccountModel> GetAccountAsync([FromRoute] string id, CancellationToken cancellationToken = default)
    {
        if (!this.HttpContext.TryGetCommerzCredentials(out var credentials))
            return null;

        this._accounts.AuthorizationToken = credentials.AuthenticationToken;
        var account = await this._accounts.GetAccountBalanceListAsync(id, cancellationToken);

        return new()
        {
            AccountId = account.AccountId,
            Iban = account.Iban,
            Bban = account.Bban,
            AccountNumberInternal = account.AccountNumberInternal,
            AccountNumberDisplay = account.AccountNumberDisplay,
            Currency = account.Currency,
            Balances = account.Balances.Select(x => new AccountBalanceModel
            {
                Type = x.BalanceType.BalanceType switch
                {
                    Common.Models.CommerzBalanceType.ClosingBooked => AccountBalanceType.ClosingBooked,
                    Common.Models.CommerzBalanceType.OpeningBooked => AccountBalanceType.OpeningBooked,
                    Common.Models.CommerzBalanceType.InterimAvailable => AccountBalanceType.InterimAvailable,
                },
                Amount = x.BalanceAmount.Amount,
                Currency = x.BalanceAmount.Currency,
                CreditLimitIncluded = x.CreditLimitIncluded,
                ReferenceDate = x.ReferenceDate,
                LastChangeDateTime = x.LastChangeDateTime,
            })
        };
    }

    [HttpGet, Route("{id}/balances")]
    public async Task<IEnumerable<AccountBalanceModel>> GetAccountBalancesAsync([FromRoute] string id, CancellationToken cancellationToken = default)
    {
        if (!this.HttpContext.TryGetCommerzCredentials(out var credentials))
            return null;

        this._accounts.AuthorizationToken = credentials.AuthenticationToken;
        var balances = await this._accounts.GetAccountAsync(id, cancellationToken);

        return balances.Balances.Select(x => new AccountBalanceModel
        {
            Type = x.BalanceType.BalanceType switch
            {
                Common.Models.CommerzBalanceType.ClosingBooked => AccountBalanceType.ClosingBooked,
                Common.Models.CommerzBalanceType.OpeningBooked => AccountBalanceType.OpeningBooked,
                Common.Models.CommerzBalanceType.InterimAvailable => AccountBalanceType.InterimAvailable,
            },
            Amount = x.BalanceAmount.Amount,
            Currency = x.BalanceAmount.Currency,
            CreditLimitIncluded = x.CreditLimitIncluded,
            ReferenceDate = x.ReferenceDate,
            LastChangeDateTime = x.LastChangeDateTime,
        });
    }
}
