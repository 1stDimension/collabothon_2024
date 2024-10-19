using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomethingFishy.Collabothon2024.API.Data.Models;
using SomethingFishy.Collabothon2024.Common;
using SomethingFishy.Collabothon2024.Common.Models;

namespace SomethingFishy.Collabothon2024.API.Controllers;

[ApiController, Authorize]
[Route("/api/v1/[controller]")]
public sealed class CorporatePaymentsController : ControllerBase
{
    private readonly ICommerzCorporatePaymentsClient _corporatePayments;

    public CorporatePaymentsController(ICommerzCorporatePaymentsClient corporatePayments)
    {
        this._corporatePayments = corporatePayments;
    }

    [HttpGet, Route("")]
    public async Task<IEnumerable<PaymentMessageModel>> GetMessagesAsync(CancellationToken cancellationToken = default)
    {
        if (!this.HttpContext.TryGetCommerzCredentials(out var credentials))
            return null;

        this._corporatePayments.AuthorizationToken = credentials.AuthenticationToken;
        var messages = await this._corporatePayments.GetMessagesAsync(cancellationToken);
        return messages.Select(x => new PaymentMessageModel
        {
            Id = x.MessageId,
            FragmentCount = x.Fragments,
            Type = x.OrderType switch
            {
                Common.Models.CommerzOrderType.C52 => PaymentMessageType.C52,
                Common.Models.CommerzOrderType.C53 => PaymentMessageType.C53,
                Common.Models.CommerzOrderType.HAC => PaymentMessageType.HAC,
            },
            Size = x.Size,
        });
    }

    [HttpPost, Route("")]
    [Consumes("application/xml")]
    public async Task<IActionResult> PostMessageAsync([FromBody] IFormFile data, CancellationToken cancellationToken = default)
    {
        if (!this.HttpContext.TryGetCommerzCredentials(out var credentials))
            return null;

        this._corporatePayments.AuthorizationToken = credentials.AuthenticationToken;
        using var stream = data.OpenReadStream();
        await this._corporatePayments.CreateMessageAsync(stream, cancellationToken);
        return this.NoContent();
    }

    [HttpGet, Route("{id}")]
    [Produces("application/xml")]
    public async Task<IActionResult> GetMessageAsync([FromRoute] string id, CancellationToken cancellationToken = default)
    {
        if (!this.HttpContext.TryGetCommerzCredentials(out var credentials))
            return this.BadRequest();

        this._corporatePayments.AuthorizationToken = credentials.AuthenticationToken;
        var messages = await this._corporatePayments.GetMessagesAsync(cancellationToken);
        var message = messages.FirstOrDefault(x => x.MessageId == id);

        this.HttpContext.Response.StatusCode = 200;
        this.HttpContext.Response.ContentType = "application/xml";
        this.HttpContext.Response.ContentLength = message.Size;

        for (var i = 0; i < message.Fragments; ++i)
            await this._corporatePayments.GetMessageAsync(id, i, this.HttpContext.Response.Body, cancellationToken);

        await this.HttpContext.Response.CompleteAsync();
        return null;
    }

    [HttpPut, Route("{id}")]
    public async Task<IActionResult> ConfirmMessageAsync([FromRoute] string id, [FromBody] PaymentMessageStatusModel status, CancellationToken cancellationToken = default)
    {
        if (!this.HttpContext.TryGetCommerzCredentials(out var credentials))
            return this.BadRequest();

        this._corporatePayments.AuthorizationToken = credentials.AuthenticationToken;
        await this._corporatePayments.SetTransferStatusAsync(id, new() { Received = status.Status switch { PaymentMessageStatus.Partial => CommerzMessageStatus.Partial, PaymentMessageStatus.Complete => CommerzMessageStatus.Complete } }, cancellationToken);
        return this.NoContent();
    }
}
