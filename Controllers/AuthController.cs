using Microsoft.AspNetCore.Mvc;
using MyWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using MyWebApi.DTOs.Auth;
using MyWebApi.Interfaces;

namespace MyWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// New account registration.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Register([FromBody] User newUser)
    {
        try
        {
            var result = await _authService.RegisterAsync(newUser);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Login with username and password
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        var token = await _authService.LoginAsync(loginRequestDto);
        
        if (token == null) 
            return Unauthorized("Invalid credentials");

        return Ok(new { token });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult AdminOnly()
    {
        return Ok("Hello Admin 👑");
    }

    [Authorize]
    [HttpGet("user")]
    public IActionResult UserOnly()
    {
        return Ok($"Hello {User.Identity?.Name} 👋");
    }

    /// <summary>
    /// Search users by keyword (Admin only).
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchUsers([FromQuery] string? keyword)
    {
        var users = await _authService.SearchUsersAsync(keyword);
        return Ok(users);
    }
}
