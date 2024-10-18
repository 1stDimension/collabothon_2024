using System.Collections.Generic;

namespace SomethingFishy.Collabothon2024.Common.Models;

// Generated using Gemini Advanced Artificial Intelligence, refined by Emzi Human Intelligence.

public sealed class CommerzSubscription
{

    public CommerzSubscriptionStatus SubscriptionStatus { get; set; }

    public IEnumerable<CommerzSubscriptionEntry> SubscriptionEntries { get; set; }
}

public sealed class CommerzSubscriptionEntry
{

    public string SubscriptionEntryId { get; set; }

    public string Iban { get; set; }

    public string ApiClientPrimaryPushUri { get; set; }

    public string ApiClientSecondaryPushUri { get; set; }
}

public sealed class CommerzSubscriptionEntryWithStatus
{

    public CommerzSubscriptionStatus SubscriptionStatus { get; set; }

    public string Iban { get; set; }

    public string ApiClientPrimaryPushUri { get; set; }

    public string ApiClientSecondaryPushUri { get; set; }
}

public sealed class CommerzSubscriptionStatusResponse
{

    public CommerzSubscriptionStatus SubscriptionStatus { get; set; }
}

public sealed class CommerzSubscriptionEntryStatusResponse
{

    public CommerzSubscriptionEntryStatus SubscriptionEntryStatus { get; set; }
}

public sealed class CommerzSubscriptionResponse
{

    public string SubscriptionId { get; set; }

    public CommerzSubscriptionStatus SubscriptionStatus { get; set; }
}

public sealed class CommerzSubscriptionEntryResponse
{

    public string SubscriptionEntryId { get; set; }

    public CommerzSubscriptionEntryStatus SubscriptionEntryStatus { get; set; }
}

public sealed class CommerzSubscriptionRequest
{

    public IEnumerable<CommerzSubscriptionEntryRequest> SubscriptionEntries { get; set; }
}

public sealed class CommerzSubscriptionEntryRequest
{

    public string ApiClientPrimaryPushUri { get; set; }

    public string ApiClientSecondaryPushUri { get; set; }

    public string Iban { get; set; }
}

public sealed class CommerzPostCertificateRequest
{

    public string Hostname { get; set; }

    public string Certificate { get; set; }
}

public enum CommerzSubscriptionStatus
{
    Valid,
    CancelledByAspsp,
    TerminatedByTpp,
}

public enum CommerzSubscriptionEntryStatus
{
    Valid,
    CancelledByAspsp,
    TerminatedByTpp,
    ServerNotReachable
}
