using MyWebApi.DTOs.Auth;
using MyWebApi.Models;

namespace MyWebApi.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(User newUser);
    Task<string?> LoginAsync(LoginRequestDto loginRequestDto);
    Task<List<User>> SearchUsersAsync(string? keyword);
}
