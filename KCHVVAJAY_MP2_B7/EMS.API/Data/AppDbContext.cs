using EMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique email constraint on Employees
        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Email)
            .IsUnique();

        // Unique username constraint on Users
        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // ── Seed Users ──────────────────────────────────────────────────────
        // IMPORTANT: These hashes are computed at application startup, not hardcoded.
        // They are generated once during OnModelCreating and baked into the migration.
        // admin123 and viewer123 are hashed with BCrypt workFactor 12.
        var adminHash  = BCrypt.Net.BCrypt.HashPassword("admin123",  workFactor: 12);
        var viewerHash = BCrypt.Net.BCrypt.HashPassword("viewer123", workFactor: 12);

        modelBuilder.Entity<AppUser>().HasData(
            new AppUser
            {
                Id = 1,
                Username = "admin",
                PasswordHash = adminHash,
                Role = "Admin",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new AppUser
            {
                Id = 2,
                Username = "viewer",
                PasswordHash = viewerHash,
                Role = "Viewer",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // ── Seed 15 Employees (identical to Mini Project 1 data.js) ────────
        modelBuilder.Entity<Employee>().HasData(
            new Employee { Id = 1,  FirstName = "Chandu",     LastName = "Ajay",      Email = "chandu.ajay@gmail.com",      Phone = "9876543210", Department = "Engineering", Designation = "Software Engineer",       Salary = 850000m,  JoinDate = new DateTime(2021, 3, 15),  Status = "Active",   CreatedAt = new DateTime(2024, 1, 1,  0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 2,  FirstName = "Sairam",     LastName = "Kolavennu", Email = "sairam.kolavennu@gmail.com", Phone = "9823456701", Department = "Marketing",   Designation = "Marketing Executive",    Salary = 620000m,  JoinDate = new DateTime(2020, 7, 1),   Status = "Active",   CreatedAt = new DateTime(2024, 1, 2,  0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 3,  FirstName = "Neha",       LastName = "Chaudhary", Email = "neha.chaudhary@gmail.com",   Phone = "9812345678", Department = "HR",          Designation = "HR Executive",           Salary = 550000m,  JoinDate = new DateTime(2019, 11, 20), Status = "Active",   CreatedAt = new DateTime(2024, 1, 3,  0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 4,  FirstName = "Rohith",     LastName = "Reddy",     Email = "rohith.reddy@gmail.com",     Phone = "9834567890", Department = "Finance",     Designation = "Financial Analyst",      Salary = 720000m,  JoinDate = new DateTime(2022, 1, 10),  Status = "Active",   CreatedAt = new DateTime(2024, 1, 4,  0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 5,  FirstName = "Ambika",     LastName = "Kumari",    Email = "ambika.kumari@gmail.com",    Phone = "9845678901", Department = "Operations",  Designation = "Operations Manager",     Salary = 950000m,  JoinDate = new DateTime(2018, 6, 5),   Status = "Active",   CreatedAt = new DateTime(2024, 1, 5,  0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 6,  FirstName = "Vikram",     LastName = "Rajputh",   Email = "vikram.rajputh@gmail.com",   Phone = "9856789012", Department = "Engineering", Designation = "Senior Developer",       Salary = 1100000m, JoinDate = new DateTime(2017, 9, 12),  Status = "Active",   CreatedAt = new DateTime(2024, 1, 6,  0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 7,  FirstName = "Anusha",     LastName = "Kapoor",    Email = "anusha.kapoor@gmail.com",    Phone = "9867890123", Department = "Marketing",   Designation = "Content Strategist",     Salary = 580000m,  JoinDate = new DateTime(2023, 2, 28),  Status = "Inactive", CreatedAt = new DateTime(2024, 1, 7,  0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 8,  FirstName = "Karthikeya", LastName = "Varma",     Email = "karthikeya.varma@gmail.com", Phone = "9878901234", Department = "Finance",     Designation = "Accounts Manager",       Salary = 800000m,  JoinDate = new DateTime(2020, 4, 17),  Status = "Active",   CreatedAt = new DateTime(2024, 1, 8,  0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 9,  FirstName = "Kavya",      LastName = "Nikitha",   Email = "kavya.nikitha@gmail.com",    Phone = "9889012345", Department = "Engineering", Designation = "Talent Acquisition Lead",Salary = 900000m,  JoinDate = new DateTime(2021, 8, 23),  Status = "Inactive", CreatedAt = new DateTime(2024, 1, 9,  0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 10, FirstName = "Mohan",      LastName = "Lal",       Email = "mohan.lal@gmail.com",        Phone = "9890123456", Department = "Operations",  Designation = "Logistics Coordinator",  Salary = 610000m,  JoinDate = new DateTime(2019, 3, 14),  Status = "Active",   CreatedAt = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 11, FirstName = "Poorna",     LastName = "Lakshmi",   Email = "poorna.lakshmi@gmail.com",   Phone = "9801234567", Department = "Marketing",   Designation = "Brand Manager",          Salary = 780000m,  JoinDate = new DateTime(2021, 11, 1),  Status = "Active",   CreatedAt = new DateTime(2024, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 12, FirstName = "Mahesh",     LastName = "Babu",      Email = "mahesh.babu@gmail.com",      Phone = "9811234568", Department = "Finance",     Designation = "Tax Consultant",         Salary = 690000m,  JoinDate = new DateTime(2022, 6, 18),  Status = "Inactive", CreatedAt = new DateTime(2024, 1, 12, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 13, FirstName = "Meera",      LastName = "Joseph",    Email = "meera.joseph@gmail.com",     Phone = "9822345679", Department = "Engineering", Designation = "QA Engineer",            Salary = 730000m,  JoinDate = new DateTime(2022, 9, 1),   Status = "Active",   CreatedAt = new DateTime(2024, 1, 13, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 14, FirstName = "Chandini",   LastName = "Priya",     Email = "chandini.priya@gmail.com",   Phone = "9833456780", Department = "Engineering", Designation = "DevOps Engineer",        Salary = 970000m,  JoinDate = new DateTime(2023, 1, 20),  Status = "Active",   CreatedAt = new DateTime(2024, 1, 14, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 15, FirstName = "Amit",       LastName = "Narayana",  Email = "amit.narayana@gmail.com",    Phone = "9844567891", Department = "Operations",  Designation = "Supply Chain Analyst",   Salary = 650000m,  JoinDate = new DateTime(2020, 10, 12), Status = "Inactive", CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
