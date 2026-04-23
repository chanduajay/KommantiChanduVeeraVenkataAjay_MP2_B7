using System.ComponentModel.DataAnnotations;

namespace EMS.API.DTOs;

// ── Auth DTOs ──────────────────────────────────────────────────────────────

public class RegisterRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    // Defaults to "Viewer" if omitted. Must be "Admin" or "Viewer".
    public string? Role { get; set; }
}

public class LoginRequestDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Username { get; set; }
    public string? Role { get; set; }
    public string? Message { get; set; }
}

// ── Employee Request DTO (server-side validation via Data Annotations) ─────

public class EmployeeRequestDto
{
    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone must be exactly 10 digits.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required.")]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required(ErrorMessage = "Designation is required.")]
    [MaxLength(150)]
    public string Designation { get; set; } = string.Empty;

    [Required(ErrorMessage = "Salary is required.")]
    [Range(1, double.MaxValue, ErrorMessage = "Salary must be a positive number.")]
    public decimal Salary { get; set; }

    [Required(ErrorMessage = "Join date is required.")]
    public DateTime JoinDate { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [RegularExpression("^(Active|Inactive)$", ErrorMessage = "Status must be Active or Inactive.")]
    public string Status { get; set; } = "Active";
}

// ── Employee Response DTO ──────────────────────────────────────────────────

public class EmployeeResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public string JoinDate { get; set; } = string.Empty;  // "yyyy-MM-dd" for JS date input
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// ── Employee List Query (search + filter + sort + pagination) ──────────────

public class EmployeeListQueryDto
{
    public string? Search { get; set; }
    public string? Department { get; set; }
    public string? Status { get; set; }
    public string SortField { get; set; } = "id";       // id | name | salary | joinDate
    public string SortDir { get; set; } = "asc";        // asc | desc
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;             // default 15 (shows all seed data), max 100
}

// ── Paged Result ───────────────────────────────────────────────────────────

public class PagedResultDto<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

// ── Dashboard DTOs ─────────────────────────────────────────────────────────

public class DashboardDto
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int InactiveEmployees { get; set; }
    public int TotalDepartments { get; set; }
    public List<DepartmentBreakdownDto> DepartmentBreakdown { get; set; } = new();
    public List<EmployeeResponseDto> RecentEmployees { get; set; } = new();
}

public class DepartmentBreakdownDto
{
    public string Dept { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Percentage { get; set; }
}
