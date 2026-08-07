using Microsoft.AspNetCore.Authorization;
using Pulse.Api.Shared;

namespace Pulse.Api.Data.Controllers.Base;

[Authorize(
    AuthenticationSchemes = ApiKeyAuthenticationDefaults.AuthenticationScheme,
    Policy = ApiKeyAuthenticationDefaults.Policy)]
public abstract class DataApiController : ApiController;