using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Interfaces;
using EMS.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EMS.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext    _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db     = db;
        _config = config;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        // Duplicate username → 409 Conflict
        var exists = await _db.Users.AnyAsync(u =>
            u.Username.ToLower() == dto.Username.Trim().ToLower());

        if (exists)
            return new AuthResponseDto
            {
                Success = false,
                Message = $"Username '{dto.Username}' is already taken. Please choose another."
            };

        // Role defaults to "Viewer" if omitted; must be Admin or Viewer
        var role = string.IsNullOrWhiteSpace(dto.Role) ? "Viewer" : dto.Role.Trim();
        if (role != "Admin" && role != "Viewer") role = "Viewer";

        // BCrypt hash — 12 rounds, never stored in plain text
        var user = new AppUser
        {
            Username     = dto.Username.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 12),
            Role         = role,
            CreatedAt    = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new AuthResponseDto
        {
            Success  = true,
            Username = user.Username,
            Role     = user.Role,
            Message  = "Registration successful."
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u =>
            u.Username.ToLower() == dto.Username.Trim().ToLower());

        // BCrypt.Verify — does not reveal which field is wrong
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid username or password."
            };

        return new AuthResponseDto
        {
            Success  = true,
            Token    = GenerateToken(user),
            Username = user.Username,
            Role     = user.Role,
            Message  = "Login successful."
        };
    }

    // JWT token generation exactly as specified in the document
    private string GenerateToken(AppUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name,           user.Username),
            new Claim(ClaimTypes.Role,           user.Role)
        };

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:             _config["Jwt:Issuer"],
            audience:           _config["Jwt:Audience"],
            claims:             claims,
            expires:            DateTime.UtcNow.AddHours(
                                    double.Parse(_config["Jwt:ExpiryHours"] ?? "8")),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
