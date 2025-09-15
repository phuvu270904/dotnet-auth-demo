using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.DTOs.Auth;
using MyWebApi.Interfaces;
using MyWebApi.Models;
using MyWebApi.Services;

namespace MyWebApi.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly JwtService _jwt;

    public AuthService(AppDbContext db, JwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<string> RegisterAsync(User newUser)
    {
        if (await _db.Users.AnyAsync(u => u.Username == newUser.Username))
        {
            throw new InvalidOperationException("Username already exists");
        }

        await _db.Users.AddAsync(newUser);
        await _db.SaveChangesAsync();

        return "User registered successfully";
    }

    public async Task<string?> LoginAsync(LoginRequestDto loginRequestDto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u =>
            u.Username == loginRequestDto.username && u.Password == loginRequestDto.password);

        if (user == null) 
            return null;

        return _jwt.GenerateToken(user.Id, user.Username, user.Role);
    }

    public async Task<List<User>> SearchUsersAsync(string? keyword)
    {
        var query = _db.Users.AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(u => u.Username.Contains(keyword));
        }

        return await query.ToListAsync();
    }
}
