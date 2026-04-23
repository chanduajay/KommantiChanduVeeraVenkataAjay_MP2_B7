using EMS.API.DTOs;
using EMS.API.Interfaces;
using EMS.API.Models;

namespace EMS.API.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repo;

    public EmployeeService(IEmployeeRepository repo)
    {
        _repo = repo;
    }

    // Map entity → response DTO
    private static EmployeeResponseDto MapToDto(Employee e) => new()
    {
        Id          = e.Id,
        FirstName   = e.FirstName,
        LastName    = e.LastName,
        Email       = e.Email,
        Phone       = e.Phone,
        Department  = e.Department,
        Designation = e.Designation,
        Salary      = e.Salary,
        JoinDate    = e.JoinDate.ToString("yyyy-MM-dd"),  // JS date input format
        Status      = e.Status,
        CreatedAt   = e.CreatedAt
    };

    public async Task<EmployeeResponseDto?> GetByIdAsync(int id)
    {
        var emp = await _repo.GetByIdAsync(id);
        return emp == null ? null : MapToDto(emp);
    }

    public async Task<PagedResultDto<EmployeeResponseDto>> GetPagedAsync(EmployeeListQueryDto query)
    {
        var (items, totalCount) = await _repo.GetPagedAsync(query);
        var pageSize = Math.Min(Math.Max(query.PageSize, 1), 100);
        var page     = Math.Max(query.Page, 1);

        return new PagedResultDto<EmployeeResponseDto>
        {
            Data       = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page       = page,
            PageSize   = pageSize
        };
    }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var total      = await _repo.GetTotalCountAsync();
        var active     = await _repo.GetActiveCountAsync();
        var inactive   = await _repo.GetInactiveCountAsync();
        var deptCounts = await _repo.GetDepartmentCountsAsync();
        var recent     = await _repo.GetRecentAsync(5);

        var breakdown = deptCounts.Select(d => new DepartmentBreakdownDto
        {
            Dept       = d.Dept,
            Count      = d.Count,
            Percentage = total > 0 ? (int)Math.Round((double)d.Count / total * 100) : 0
        }).ToList();

        return new DashboardDto
        {
            TotalEmployees      = total,
            ActiveEmployees     = active,
            InactiveEmployees   = inactive,
            TotalDepartments    = deptCounts.Count,
            DepartmentBreakdown = breakdown,
            RecentEmployees     = recent.Select(MapToDto).ToList()
        };
    }

    public async Task<(EmployeeResponseDto? Employee, string? Error)> AddAsync(EmployeeRequestDto dto)
    {
        // Check email uniqueness — returns 409 if taken
        if (await _repo.EmailExistsAsync(dto.Email.Trim(), null))
            return (null, "email_conflict");

        var entity = new Employee
        {
            FirstName   = dto.FirstName.Trim(),
            LastName    = dto.LastName.Trim(),
            Email       = dto.Email.Trim().ToLower(),
            Phone       = dto.Phone.Trim(),
            Department  = dto.Department.Trim(),
            Designation = dto.Designation.Trim(),
            Salary      = dto.Salary,
            JoinDate    = dto.JoinDate,
            Status      = dto.Status
        };

        var created = await _repo.AddAsync(entity);
        return (MapToDto(created), null);
    }

    public async Task<(EmployeeResponseDto? Employee, string? Error)> UpdateAsync(int id, EmployeeRequestDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null)
            return (null, "not_found");

        // Allow same email for the same employee (excludes current id from check)
        if (await _repo.EmailExistsAsync(dto.Email.Trim(), id))
            return (null, "email_conflict");

        existing.FirstName   = dto.FirstName.Trim();
        existing.LastName    = dto.LastName.Trim();
        existing.Email       = dto.Email.Trim().ToLower();
        existing.Phone       = dto.Phone.Trim();
        existing.Department  = dto.Department.Trim();
        existing.Designation = dto.Designation.Trim();
        existing.Salary      = dto.Salary;
        existing.JoinDate    = dto.JoinDate;
        existing.Status      = dto.Status;

        var updated = await _repo.UpdateAsync(existing);
        return (MapToDto(updated), null);
    }

    public async Task<bool> DeleteAsync(int id)
        => await _repo.DeleteAsync(id);
}
