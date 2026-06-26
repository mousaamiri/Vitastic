using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Vitastic.Api.Features.Base;

[Route("api/[controller]")]
[ApiController]
internal class BaseApiController : ControllerBase
{
}
