using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Services;
using Contracts.DTOs;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class TokenController : ControllerBase
{
    private readonly AppUserService _appUserService;

    public TokenController(AppUserService appUserService)
    {
        _appUserService = appUserService;
    }
    
    /// <summary>
    /// Create new JWT
    /// </summary>
    /// <response code="400">Bad request</response>
    /// <response code="200">Token was created with success</response>
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(AppUser), StatusCodes.Status200OK)]
    public async Task<IActionResult> AuthenticateUser([FromBody] GetAppUserDTO getAppUserDto)
    {
        if (string.IsNullOrWhiteSpace(getAppUserDto.Email) || string.IsNullOrWhiteSpace(getAppUserDto.Password))
            return BadRequest("Invalid Email or Password");
        
        var response = await _appUserService.GetUserWithPassword(getAppUserDto);

        if (response is null)
            return BadRequest("Invalid Email or Password");

        var claimsArray = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, Environment.GetEnvironmentVariable("JWT_SUBJECT") ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)),
            new("userId", response.Id.ToString()),
            new("email", response.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")
                                                                  ?? throw new Exception()));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
            audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            claims: claimsArray,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: credentials);

        var serializedToken = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(serializedToken);
    }
}