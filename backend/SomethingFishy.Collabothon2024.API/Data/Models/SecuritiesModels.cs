using System;
using System.Collections.Generic;

namespace SomethingFishy.Collabothon2024.API.Data.Models;

public sealed class SecuritiesPortfolioModel
{
    public DateOnly CreatedAt { get; init; }
    public DateOnly EffectiveDate { get; init; }
    public string SecuritiesAccountId { get; init; }
    public SecuritiesValueModel TotalValue { get; init; }
    public IEnumerable<SecuritiesPositionModel> Positions { get; init; }
}

public sealed class SecuritiesTransactionsModel
{
    public string SecuritiesAccountId { get; init; }
    public IEnumerable<SecuritiesTransactionModel> Transactions { get; init; }
}

public sealed class SecuritiesPositionModel
{
    public SecuritiesValueModel AccruedInterest { get; init; }
    public SecuritiesCurrentPriceModel CurrentPrice { get; init; }
    public SecuritiesInitialPriceModel InitialPrice { get; init; }
    public DateOnly LastTraded { get; init; }
    public SecuritiesValueModel PaidAccruedInterest { get; init; }
    public SecuritiesValueModel Payout { get; init; }
    public string PositionId { get; init; }
    public SecuritiesValueModel Quantity { get; init; }
    public SecuritiesMasterdataModel Masterdata { get; init; }
}

public sealed class SecuritiesTransactionModel
{
    public SecuritiesValueModel AccruedInterest { get; init; }
    public SecuritiesBlockModel BlockInfo { get; init; }
    public DateOnly BookingDate { get; init; }
    public SecuritiesCancellationModel Cancellation { get; init; }
    public IEnumerable<SecuritiesCostModel> Costs { get; init; }
    public string Depository { get; init; }
    public decimal ExchangeRate { get; init; }
    public string PositionId { get; init; }
    public SecuritiesValueModel Price { get; init; }
    public SecuritiesMasterdataLiteModel Masterdata { get; init; }
    public SecuritiesValueModel Size { get; init; }
    public DateOnly TradingDate { get; init; }
    public string TradingPlatform { get; init; }
    public DateTimeOffset TradingTimestamp { get; init; }
    public string TransactionId { get; init; }
    public SecurityTransactionType TransactionType { get; init; }
    public DateOnly ValutaDate { get; init; }
    public SecuritiesSettlementAccountModel SettlementAccount { get; init; }
    public SecuritiesValueModel MarketValue { get; init; }
    public SecuritiesValueModel ActualAmount { get; init; }
    public string ExternalOrderNumber { get; init; }
    public string SettlementNumber { get; init; }
    public int ExecutionNumber { get; init; }
    public string ClientOrderId { get; init; }
    public string TransactionDetailedType { get; init; }
    public IEnumerable<SecuritiesTaxModel> Taxes { get; init; }
}

public sealed class SecuritiesCurrentPriceModel
{
    public SecuritiesValueModel Price { get; init; }
    public decimal ExchangeRate { get; init; }
    public DateOnly ExchangeRateDate { get; init; }
    public DateOnly QuoteDate { get; init; }
}

public sealed class SecuritiesInitialPriceModel
{
    public SecuritiesValueModel Price { get; init; }
    public decimal ExchangeRate { get; init; }
}

public sealed class SecuritiesMasterdataModel
{
    public string Isin { get; init; }
    public string Wkn { get; init; }
    public string Name { get; init; }
    public string NotationType { get; init; }
    public string Currency { get; init; }
    public DateOnly MaturityDate { get; init; }
    public SecurityVote Vote { get; init; }
}

public sealed class SecuritiesMasterdataLiteModel
{
    public string Isin { get; init; }
    public string Wkn { get; init; }
    public string Name { get; init; }
    public string NotationType { get; init; }
}

public sealed class SecuritiesTaxModel
{
    public string Type { get; init; }
    public string Description { get; init; }
    public SecuritiesValueModel Value { get; init; }
}

public sealed class SecuritiesCostModel
{
    public string Description { get; init; }
    public SecuritiesValueModel Cost { get; init; }
}

public sealed class SecuritiesValueModel
{
    public decimal Value { get; init; }
    public string Currency { get; init; }
}

public sealed class SecuritiesBlockModel
{
    public string Reason { get; init; }
    public DateOnly Until { get; init; }
}

public sealed class SecuritiesCancellationModel
{
    public string TransactionId { get; init; }
    public bool IsCancellation { get; init; }
}

public sealed class SecuritiesSettlementAccountModel
{
    public string Id { get; init; }
    public string Iban { get; init; }
    public string Currency { get; init; }
}

public enum SecurityVote
{
    Buy,
    Sell,
    Hold,
}

public enum SecurityTransactionType
{
    Purchase,
    Sale,
    Delivery,
    Deposit,
    Maturity,
}
