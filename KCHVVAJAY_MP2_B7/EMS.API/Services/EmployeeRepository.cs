using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Interfaces;
using EMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.API.Services;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _db;

    public EmployeeRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Employee?> GetByIdAsync(int id)
        => await _db.Employees.FindAsync(id);

    public async Task<(List<Employee> Items, int TotalCount)> GetPagedAsync(EmployeeListQueryDto query)
    {
        var q = _db.Employees.AsQueryable();

        // ── Search: WHERE (FirstName + ' ' + LastName LIKE '%term%' OR Email LIKE '%term%')
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(e =>
                (e.FirstName.ToLower() + " " + e.LastName.ToLower()).Contains(term) ||
                e.Email.ToLower().Contains(term));
        }

        // ── Department filter: exact match WHERE Department = @dept
        if (!string.IsNullOrWhiteSpace(query.Department) &&
            !query.Department.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            q = q.Where(e => e.Department == query.Department);
        }

        // ── Status filter: exact match WHERE Status = @status
        if (!string.IsNullOrWhiteSpace(query.Status) &&
            !query.Status.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            q = q.Where(e => e.Status == query.Status);
        }

        // ── Sort: ORDER BY executes in SQL
        q = (query.SortField?.ToLower(), query.SortDir?.ToLower()) switch
        {
            ("id",       "desc") => q.OrderByDescending(e => e.Id),
            ("id",       _)      => q.OrderBy(e => e.Id),
            ("name",     "desc") => q.OrderByDescending(e => e.LastName).ThenByDescending(e => e.FirstName),
            ("name",     _)      => q.OrderBy(e => e.LastName).ThenBy(e => e.FirstName),
            ("salary",   "desc") => q.OrderByDescending(e => e.Salary),
            ("salary",   _)      => q.OrderBy(e => e.Salary),
            ("joindate", "desc") => q.OrderByDescending(e => e.JoinDate),
            ("joindate", _)      => q.OrderBy(e => e.JoinDate),
            _                    => q.OrderBy(e => e.Id)   // default: ID order
        };

        var totalCount = await q.CountAsync();

        // ── Pagination: Skip/Take executes in SQL
        var pageSize = Math.Min(Math.Max(query.PageSize, 1), 100);
        var page     = Math.Max(query.Page, 1);
        var items    = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    // Recent employees: last 5 added, ordered by CreatedAt desc then Id desc
    public async Task<List<Employee>> GetRecentAsync(int count)
        => await _db.Employees
            .OrderByDescending(e => e.CreatedAt)
            .ThenByDescending(e => e.Id)
            .Take(count)
            .ToListAsync();

    public async Task<int> GetTotalCountAsync()
        => await _db.Employees.CountAsync();

    public async Task<int> GetActiveCountAsync()
        => await _db.Employees.CountAsync(e => e.Status == "Active");

    public async Task<int> GetInactiveCountAsync()
        => await _db.Employees.CountAsync(e => e.Status == "Inactive");

    // Department breakdown: GROUP BY Department, sorted alphabetically
    public async Task<List<(string Dept, int Count)>> GetDepartmentCountsAsync()
        => await _db.Employees
            .GroupBy(e => e.Department)
            .Select(g => new { Dept = g.Key, Count = g.Count() })
            .OrderBy(x => x.Dept)
            .Select(x => ValueTuple.Create(x.Dept, x.Count))
            .ToListAsync();

    public async Task<bool> EmailExistsAsync(string email, int? excludeId)
        => await _db.Employees.AnyAsync(e =>
            e.Email.ToLower() == email.ToLower() &&
            (excludeId == null || e.Id != excludeId));

    public async Task<Employee> AddAsync(Employee employee)
    {
        employee.CreatedAt = DateTime.UtcNow;
        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee> UpdateAsync(Employee employee)
    {
        _db.Employees.Update(employee);
        await _db.SaveChangesAsync();
        return employee;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var emp = await _db.Employees.FindAsync(id);
        if (emp == null) return false;
        _db.Employees.Remove(emp);
        await _db.SaveChangesAsync();
        return true;
    }
}
