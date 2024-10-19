using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SomethingFishy.Collabothon2024.API.Controllers;

[ApiController, Authorize]
[Route("/api/v1/[controller]")]
public sealed class TestController : ControllerBase
{
    public TestController()
    { }

    [HttpGet, Route("")]
    public async Task<IActionResult> TestAsync(CancellationToken cancellationToken = default)
        => this.NoContent();
}
