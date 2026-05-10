using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pulse.Api.Mgmt.Controllers.Base;

namespace Pulse.Api.Mgmt.Controllers;

[Route("mgmt/v1/applications")]
public class ApplicationController : MgmtApiController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        return Ok();
    }
}