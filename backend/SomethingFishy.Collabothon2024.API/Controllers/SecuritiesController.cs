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
public sealed class SecuritiesController : ControllerBase
{
    private readonly ICommerzSecuritiesClient _securities;

    public SecuritiesController(ICommerzSecuritiesClient securities)
    {
        this._securities = securities;
    }

    [HttpGet, Route("accounts")]
    public async Task<IEnumerable<string>> GetSecuritiesAccountListAsync(CancellationToken cancellationToken = default)
    {
        if (!this.HttpContext.TryGetCommerzCredentials(out var credentials))
            return null;

        this._securities.AuthorizationToken = credentials.AuthenticationToken;
        var accounts = await this._securities.GetSecuritiesAccountsAsync(cancellationToken);
        return accounts.SecuritiesAccountIds.Select(x => x.PseudonymizedAccountId);
    }

    [HttpGet, Route("accounts/{id}/portfolio")]
    public async Task<SecuritiesPortfolioModel> GetPortfolioAsync([FromRoute] string id, CancellationToken cancellationToken = default)
    {
        if (!this.HttpContext.TryGetCommerzCredentials(out var credentials))
            return null;

        this._securities.AuthorizationToken = credentials.AuthenticationToken;
        var portfolio = await this._securities.GetSecuritiesPortfolioAsync(id, cancellationToken: cancellationToken);
        return new()
        {
            CreatedAt = portfolio.CreationDate,
            EffectiveDate = portfolio.EffectiveDate,
            SecuritiesAccountId = portfolio.SecuritiesAccountId,
            TotalValue = new()
            {
                Value = portfolio.TotalValue.Amount,
                Currency = portfolio.TotalValue.Currency,
            },
            Positions = portfolio.Positions.Select(x => new SecuritiesPositionModel
            {
                AccruedInterest = new()
                {
                    Value = x.AccruedInterest.Amount,
                    Currency = x.AccruedInterest.Currency,
                },
                CurrentPrice = new()
                {
                    Price = new()
                    {
                        Value = x.CurrentPrice.Price.Amount,
                        Currency = x.CurrentPrice.Price.Unit,
                    },
                    ExchangeRate = x.CurrentPrice.ExchangeRate,
                    ExchangeRateDate = x.CurrentPrice.ExchangeRateDate,
                    QuoteDate = x.CurrentPrice.QuoteDate,
                },
                InitialPrice = new()
                {
                    Price = new()
                    {
                        Value = x.InitialPrice.Price.Amount,
                        Currency = x.InitialPrice.Price.Unit,
                    },
                    ExchangeRate = x.InitialPrice.ExchangeRate,
                },
                LastTraded = x.LastTradeDate,
                PaidAccruedInterest = new()
                {
                    Value = x.PayedAccruedInterest.Amount,
                    Currency = x.PayedAccruedInterest.Currency,
                },
                Payout = new()
                {
                    Value = x.Payout.Amount,
                    Currency = x.Payout.Currency,
                },
                PositionId = x.PositionId,
                Quantity = new()
                {
                    Value = x.Quantity.Amount,
                    Currency = x.Quantity.Unit,
                },
                Masterdata = new()
                {
                    Isin = x.Masterdata.SecuritiesMasterdata.Isin,
                    Wkn = x.Masterdata.SecuritiesMasterdata.Wkn,
                    Name = x.Masterdata.SecuritiesMasterdata.Name,
                    NotationType = x.Masterdata.SecuritiesMasterdata.NotationType,
                    Currency = x.Masterdata.Currency,
                    MaturityDate = x.Masterdata.MaturityDate,
                    Vote = x.Masterdata.Vote switch
                    {
                        Common.Models.CommerzSecurityVote.Buy => SecurityVote.Buy,
                        Common.Models.CommerzSecurityVote.Sell => SecurityVote.Sell,
                        Common.Models.CommerzSecurityVote.Hold => SecurityVote.Hold,
                    },
                },
            }),
        };
    }

    [HttpGet, Route("accounts/{id}/transactions")]
    public async Task<SecuritiesTransactionsModel> GetTransactionsAsync([FromRoute] string id, [FromQuery] int limit = 25, CancellationToken cancellationToken = default)
    {
        if (!this.HttpContext.TryGetCommerzCredentials(out var credentials))
            return null;

        this._securities.AuthorizationToken = credentials.AuthenticationToken;
        var transactions = await this._securities.GetTransactionsAsync(id, limit: limit, cancellationToken: cancellationToken);
        return new()
        {
            SecuritiesAccountId = transactions.SecuritiesAccountId,
            Transactions = transactions.Transactions.Select(x => new SecuritiesTransactionModel
            {
                AccruedInterest = new()
                {
                    Value = x.AccruedInterest.Amount,
                    Currency = x.AccruedInterest.Currency,
                },
                BlockInfo = new()
                {
                    Reason = x.BlockInfo.BlockText,
                    Until = x.BlockInfo.BlockTo,
                },
                BookingDate = x.BookingDate,
                Cancellation = new()
                {
                    TransactionId = x.CancellationInfo.CancelledTransactionId,
                    IsCancellation = x.CancellationInfo.IsCancelation,
                },
                Costs = x.Costs.Select(y => new SecuritiesCostModel
                {
                    Cost = new()
                    {
                        Value = y.Money.Amount,
                        Currency = y.Money.Currency,
                    },
                    Description = y.CostDescription,
                }),
                Depository = x.Depository,
                ExchangeRate = x.ExchangeRate,
                PositionId = x.PositionId,
                Price = new()
                {
                    Value = x.Price.Amount,
                    Currency = x.Price.Unit,
                },
                Masterdata = new()
                {
                    Isin = x.Masterdata.Isin,
                    Wkn = x.Masterdata.Wkn,
                    Name = x.Masterdata.Name,
                    NotationType = x.Masterdata.NotationType,
                },
                Size = new()
                {
                    Value = x.Size.Amount,
                    Currency = x.Size.Unit,
                },
                TradingDate = x.TradingDate,
                TradingPlatform = x.TradingPlatform,
                TradingTimestamp = x.TradingTimestamp,
                TransactionId = x.TransactionId,
                TransactionType = x.TransactionType.Name switch
                {
                    Common.Models.CommerzSecurityTransactionType.Purchase => SecurityTransactionType.Purchase,
                    Common.Models.CommerzSecurityTransactionType.Sale => SecurityTransactionType.Sale,
                    Common.Models.CommerzSecurityTransactionType.Delivery => SecurityTransactionType.Delivery,
                    Common.Models.CommerzSecurityTransactionType.Deposit => SecurityTransactionType.Deposit,
                    Common.Models.CommerzSecurityTransactionType.Maturity => SecurityTransactionType.Maturity,
                },
                ValutaDate = x.ValutaDate,
                SettlementAccount = new()
                {
                    Id = x.SettlementAccount,
                    Iban = x.SettlementAccountReference.Iban,
                    Currency = x.SettlementAccountReference.Currency,
                },
                MarketValue = new()
                {
                    Value = x.MarketValue.Amount,
                    Currency = x.MarketValue.Currency,
                },
                ActualAmount = new()
                {
                    Value = x.MarketValue.Amount,
                    Currency = x.MarketValue.Currency,
                },
                ExternalOrderNumber = x.ExternalOrderNumber,
                SettlementNumber = x.SettlementNumber,
                ExecutionNumber = x.ExecutionNumber,
                ClientOrderId = x.ClientOrderId,
                TransactionDetailedType = x.TransactionDetailedType,
                Taxes = x.TaxDetails.Taxes.Select(y => new SecuritiesTaxModel
                {
                    Value = new()
                    {
                        Value = y.Amount.Amount,
                        Currency = y.Amount.Currency,
                    },
                    Description = y.TaxTypeDescription["en"],
                    Type = y.TaxType,
                }),
            }),
        };
    }
}
