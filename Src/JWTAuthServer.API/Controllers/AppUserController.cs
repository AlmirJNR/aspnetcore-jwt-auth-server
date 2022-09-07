using API.Services;
using Contracts.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class AppUserController : ControllerBase
{
    private readonly AppUserService _appUserService;
    private const int MinimumPasswordSize = 8;

    public AppUserController(AppUserService appUserService)
    {
        _appUserService = appUserService;
    }
    
    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <response code="400">Bad request</response>
    /// <response code="201">New user has been created</response>
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> AddNew([FromBody] CreateAppUserDTO createAppUserDto)
    {
        if (string.IsNullOrWhiteSpace(createAppUserDto.Email))
            return BadRequest("Insert a valid email");


        if (string.IsNullOrWhiteSpace(createAppUserDto.Password))
            return BadRequest("Insert a valid password");


        if (createAppUserDto.Password.Length < MinimumPasswordSize)
            return BadRequest("Password must be greater than or equals 8");


        var (_, response) = await _appUserService.AddNew(createAppUserDto);

        return response switch
        {
            StatusCodes.Status400BadRequest => BadRequest("User email already exists"),
            StatusCodes.Status201Created => Created("", null),
            _ => Conflict()
        };
    }

    private readonly record struct UserPassword(string Password);
    
    /// <summary>
    /// Get user password
    /// </summary>
    /// <response code="400">Bad request</response>
    /// <response code="401">User has yet to be authenticated</response>
    /// <response code="200">Password retrieved with success</response>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(UserPassword), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPassword([FromRoute] Guid userId)
    {
        var claimsIdentity = HttpContext.User;
        var claims = claimsIdentity.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);

        var userIdResponse = claims.TryGetValue("userId", out var jwtUserId);
        if (!userIdResponse || string.IsNullOrWhiteSpace(jwtUserId))
            return BadRequest();

        if (jwtUserId != userId.ToString())
            return Unauthorized();

        var user = await _appUserService.GetUserById(userId);
        if (user is null)
            return BadRequest();

        return Ok(new UserPassword(user.Password));
    }
}