using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Models;
using EMS.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace EMS.Tests;

[TestFixture]
public class AuthServiceTests
{
    private AppDbContext   _db     = null!;
    private AuthService    _svc    = null!;
    private IConfiguration _config = null!;

    [SetUp]
    public void Setup()
    {
        // InMemory database — no real SQL Server needed for tests
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);

        // Mock IConfiguration for JWT settings
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Jwt:Key"])         .Returns("TestSecretKey_32Chars_ForNUnit!!");
        mockConfig.Setup(c => c["Jwt:Issuer"])      .Returns("EMS.API");
        mockConfig.Setup(c => c["Jwt:Audience"])    .Returns("EMS.Client");
        mockConfig.Setup(c => c["Jwt:ExpiryHours"]).Returns("8");
        _config = mockConfig.Object;

        _svc = new AuthService(_db, _config);
    }

    [TearDown]
    public void TearDown()
    {
        _db.Dispose();
    }

    // ── RegisterAsync ─────────────────────────────────────────────────────────

    [Test]
    public async Task RegisterAsync_NewUsername_ReturnsSuccess()
    {
        var result = await _svc.RegisterAsync(new RegisterRequestDto
        {
            Username = "newuser",
            Password = "password123",
            Role     = "Viewer"
        });

        Assert.That(result.Success,  Is.True);
        Assert.That(result.Username, Is.EqualTo("newuser"));
        Assert.That(result.Role,     Is.EqualTo("Viewer"));
    }

    [Test]
    public async Task RegisterAsync_DefaultsToViewer_WhenRoleOmitted()
    {
        var result = await _svc.RegisterAsync(new RegisterRequestDto
        {
            Username = "testviewer",
            Password = "password123"
            // Role omitted — must default to Viewer
        });

        Assert.That(result.Success, Is.True);
        Assert.That(result.Role,    Is.EqualTo("Viewer"));
    }

    [Test]
    public async Task RegisterAsync_DuplicateUsername_ReturnsFailure()
    {
        // Seed a user first
        _db.Users.Add(new AppUser
        {
            Username     = "existinguser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass123"),
            Role         = "Admin"
        });
        await _db.SaveChangesAsync();

        var result = await _svc.RegisterAsync(new RegisterRequestDto
        {
            Username = "existinguser",
            Password = "differentpass",
            Role     = "Viewer"
        });

        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("already taken"));
    }

    // ── LoginAsync ────────────────────────────────────────────────────────────

    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        // Arrange — seed a user with known hashed password (low workFactor for test speed)
        _db.Users.Add(new AppUser
        {
            Username     = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123", workFactor: 4),
            Role         = "Admin"
        });
        await _db.SaveChangesAsync();

        var result = await _svc.LoginAsync(new LoginRequestDto
        {
            Username = "admin",
            Password = "admin123"
        });

        Assert.That(result.Success,  Is.True);
        Assert.That(result.Token,    Is.Not.Null.And.Not.Empty);
        Assert.That(result.Username, Is.EqualTo("admin"));
        Assert.That(result.Role,     Is.EqualTo("Admin"));
    }

    [Test]
    public async Task LoginAsync_WrongPassword_ReturnsFailure()
    {
        _db.Users.Add(new AppUser
        {
            Username     = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123", workFactor: 4),
            Role         = "Admin"
        });
        await _db.SaveChangesAsync();

        var result = await _svc.LoginAsync(new LoginRequestDto
        {
            Username = "admin",
            Password = "wrongpassword"
        });

        Assert.That(result.Success, Is.False);
        Assert.That(result.Token,   Is.Null);
        Assert.That(result.Message, Is.EqualTo("Invalid username or password."));
    }

    [Test]
    public async Task LoginAsync_NonExistentUser_ReturnsFailure()
    {
        var result = await _svc.LoginAsync(new LoginRequestDto
        {
            Username = "nobody",
            Password = "pass"
        });

        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Is.EqualTo("Invalid username or password."));
    }

    [Test]
    public async Task LoginAsync_CaseInsensitiveUsername_ReturnsSuccess()
    {
        _db.Users.Add(new AppUser
        {
            Username     = "Admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123", workFactor: 4),
            Role         = "Admin"
        });
        await _db.SaveChangesAsync();

        // Login with lowercase — must still succeed
        var result = await _svc.LoginAsync(new LoginRequestDto
        {
            Username = "admin",
            Password = "admin123"
        });

        Assert.That(result.Success, Is.True);
    }
}
