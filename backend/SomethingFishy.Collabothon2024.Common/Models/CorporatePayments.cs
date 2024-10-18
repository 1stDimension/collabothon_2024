namespace SomethingFishy.Collabothon2024.Common.Models;

// Generated using Gemini Advanced Artificial Intelligence, refined by Emzi Human Intelligence.

public sealed class CommerzCcscMessage
{

    /// <summary>
    /// uuid of the corresponding file that has to be fetched with separate request(s).
    /// </summary>
    public string MessageId { get; set; }

    /// <summary>
    /// | EBICS OrderTypes |||
    /// | ----| ------- | --------------- |
    /// | C52 | CAMT052 | camt.052.001.02 |
    /// | C53 | CAMT053 | camt.053.001.02 |
    /// | HAC | PAIN002 | pain.002.001.03 |
    /// </summary>
    public OrderType OrderType { get; set; }

    /// <summary>
    /// The amount of fragments needed for complete download of the file.
    /// </summary>
    public int Fragments { get; set; }

    /// <summary>
    /// The size of the message
    /// </summary>
    public int Size { get; set; }
}

public enum OrderType
{
    C52,
    C53,
    HAC,
}
