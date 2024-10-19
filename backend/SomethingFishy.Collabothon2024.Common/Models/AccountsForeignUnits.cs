using System;
using System.Collections.Generic;

namespace SomethingFishy.Collabothon2024.Common.Models;

// Generated using Gemini Advanced Artificial Intelligence, refined by Emzi Human Intelligence.

public sealed class CommerzAccount
{
    /// <summary>
    /// Account identifier. The value is pseudonymised.
    /// </summary>
    public string AccountId { get; set; }

    /// <summary>
    /// IBAN for this account.
    /// </summary>
    public string Iban { get; set; }

    /// <summary>
    /// ISO 20022 Basic Bank Account Number (BBAN). Corresponds to iban without country code (digits 1-2) and checksum (digits 3-4).
    /// </summary>
    public string Bban { get; set; }

    /// <summary>
    /// Account number, internal format. This is the 'technical' account number representation and is to be used for internal storage and communication between applications.
    /// </summary>
    public string AccountNumberInternal { get; set; }

    /// <summary>
    /// Account number, display format. This is the representation of the account number from the customer's point of view and is to be used for display purposes or rendering documents.
    /// </summary>
    public string AccountNumberDisplay { get; set; }

    /// <summary>
    /// ISO 4217 currency code.
    /// </summary>
    public string Currency { get; set; }

    public CommerzCustomerAgreementReference CustomerAgreementReference { get; set; }

    public IEnumerable<CommerzBalance> Balances { get; set; }
}

public sealed class CommerzAccountBalances
{

    public CommerzAccountReference Account { get; set; }

    public IEnumerable<CommerzBalance> Balances { get; set; }
}

public sealed class CommerzAccountList
{

    public IEnumerable<CommerzAccount> Accounts { get; set; }
}

public sealed class CommerzAccountReference
{

    /// <summary>
    /// Account identifier. This is the data element to be used as path parameter 'account-id' or query parameter 'accountId' when requesting data for a dedicated account.
    /// </summary>
    public string AccountId { get; set; }

    /// <summary>
    /// IBAN for this account.
    /// </summary>
    public string Iban { get; set; }

    /// <summary>
    /// ISO 4217 currency code.
    /// </summary>
    public string Currency { get; set; }
}

public sealed class CommerzCustomerAgreementReference
{

    /// <summary>
    /// Customer Agreement identifier, pseudonymised value.
    /// </summary>
    public string CustomerAgreementId { get; set; }

    /// <summary>
    /// Customer agreement number.
    /// </summary>
    public string CustomerAgreementNumber { get; set; }
}

public sealed class CommerzBalanceAmount
{

    /// <summary>
    /// The amount given with fractional digits, where fractions must be compliant to the currency definition.
    /// Up to 14 significant figures, negative amounts are signed by minus.
    /// The decimal separator is a dot.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// ISO 4217 currency code.
    /// </summary>
    public string Currency { get; set; }
}

public sealed class CommerzBalance
{

    public CommerzBalanceTypeWrapper BalanceType { get; set; }

    public CommerzBalanceAmount BalanceAmount { get; set; }

    /// <summary>
    /// Indicates if the credit limit of the corresponding account is included in the calculation of the balance.
    /// Applicable (true) for balanceType 'authorized' only.
    /// </summary>
    public bool CreditLimitIncluded { get; set; }

    /// <summary>
    /// Reference date of this balance.
    /// </summary>
    public DateOnly ReferenceDate { get; set; }

    /// <summary>
    /// Timestamp of last 'booked' transaction included in this balance.
    /// </summary>
    public DateTimeOffset LastChangeDateTime { get; set; }
}

public sealed class CommerzBalanceTypeWrapper
{
    public CommerzBalanceType BalanceType { get; set; }
}

public enum CommerzBalanceType
{
    ClosingBooked,
    OpeningBooked,
    InterimAvailable,
    Expected,
    Authorised,
    ForwardAvailable,
}
