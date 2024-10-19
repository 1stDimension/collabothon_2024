using System;
using System.Collections.Generic;

namespace SomethingFishy.Collabothon2024.API.Data.Models;

public sealed class AccountSummaryModel
{
    public string AccountId { get; init; }
    public string AccountNumberDisplay { get; init; }
    public string Currency { get; init; }
}

public sealed class AccountModel
{
    public string AccountId { get; init; }
    public string Iban { get; init; }
    public string Bban { get; init; }
    public string AccountNumberInternal { get; init; }
    public string AccountNumberDisplay { get; init; }
    public string Currency { get; init; }
    public IEnumerable<AccountBalanceModel> Balances { get; init; }
}

public sealed class AccountBalanceModel
{
    public AccountBalanceType Type { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; }
    public bool CreditLimitIncluded { get; init; }
    public DateOnly ReferenceDate { get; init; }
    public DateTimeOffset LastChangeDateTime { get; init; }
}

public enum AccountBalanceType
{
    ClosingBooked,
    OpeningBooked,
    InterimAvailable,
}
