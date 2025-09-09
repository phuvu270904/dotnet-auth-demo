using Microsoft.AspNetCore.Mvc;
using MyWebApi.Models;
using MyWebApi.Services;
using MyWebApi.Data;
using Microsoft.AspNetCore.Authorization;
using MyWebApi.DTOs.Auth;
using Microsoft.EntityFrameworkCore;

namespace MyWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwt;
    private readonly AppDbContext _db;

    public AuthController(JwtService jwt, AppDbContext db)
    {
        _jwt = jwt;
        _db = db;
    }

    /// <summary>
    /// New account registration.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Register([FromBody] User newUser)
    {
        if (await _db.Users.AnyAsync(u => u.Username == newUser.Username))
        {
            return BadRequest("Username already exists");
        }

        await _db.Users.AddAsync(newUser);
        await _db.SaveChangesAsync();

        return Ok("User registered successfully");
    }

    
    /// <summary>
    /// Login with username and password
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u =>
            u.Username == loginRequestDto.username && u.Password == loginRequestDto.password);

        if (user == null) return Unauthorized("Invalid credentials");

        var token = _jwt.GenerateToken(user.Username, user.Role);
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
        var query = _db.Users.AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(u => u.Username.Contains(keyword));
        }

        return Ok(await query.ToListAsync());
    }
}
