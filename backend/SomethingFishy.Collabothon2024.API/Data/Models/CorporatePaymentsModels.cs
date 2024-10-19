namespace SomethingFishy.Collabothon2024.API.Data.Models;

public sealed class PaymentMessageModel
{
    public string Id { get; init; }
    public PaymentMessageType Type { get; init; }
    public int FragmentCount { get; init; }
    public int Size { get; init; }
}

public sealed class PaymentMessageStatusModel
{
    public PaymentMessageStatus Status { get; set; }
}

public enum PaymentMessageType
{
    C52,
    C53,
    HAC,
}

public enum PaymentMessageStatus
{
    Partial,
    Complete,
}
