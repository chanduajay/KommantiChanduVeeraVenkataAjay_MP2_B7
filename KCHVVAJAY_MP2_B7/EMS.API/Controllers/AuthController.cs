using EMS.API.DTOs;
using EMS.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers;

/// <summary>
/// Handles user registration and login.
/// All endpoints are decorated with [AllowAnonymous] — no JWT token required.
/// Passwords are hashed with BCrypt (workFactor: 12) before storage.
/// On successful login a signed JWT token is returned containing the username,
/// role, and expiry claims.
/// </summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>Injects the auth service via constructor injection.</summary>
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// POST /api/auth/register
    /// Registers a new user account.
    /// Accepts: { username, password, role? }
    /// Role defaults to "Viewer" if omitted. Valid values: "Admin" or "Viewer".
    /// Passwords are hashed with BCrypt (12 rounds) — never stored in plain text.
    /// Returns 201 Created on success.
    /// Returns 409 Conflict with a descriptive message if the username is already taken.
    /// Returns 400 Bad Request if validation fails (e.g. password shorter than 6 chars).
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(dto);
        return result.Success
            ? StatusCode(StatusCodes.Status201Created, result)
            : Conflict(result);
    }

    /// <summary>
    /// POST /api/auth/login
    /// Authenticates a user and returns a signed JWT token.
    /// Accepts: { username, password }
    /// BCrypt.Verify() is used to compare the submitted password against the stored hash.
    /// Returns 200 OK with { success, token, username, role, message } on success.
    /// Returns 401 Unauthorized with { success: false, message } on failure.
    /// The error message deliberately does not reveal which field is wrong (security best practice).
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.LoginAsync(dto);
        return result.Success ? Ok(result) : Unauthorized(result);
    }
}
