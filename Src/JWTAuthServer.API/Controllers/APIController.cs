using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]/v1")]
public class APIController : ControllerBase
{
    /// <summary>
    /// Default API check-status response
    /// </summary>
    /// <response code="200">Default API check-status response</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get() => Ok("JWT Auth Server API is OK");
}