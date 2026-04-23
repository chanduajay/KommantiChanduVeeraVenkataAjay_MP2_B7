using EMS.API.DTOs;
using EMS.API.Models;

namespace EMS.API.Interfaces;

// Data access contract — Moq mocks this in unit tests
public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id);
    Task<(List<Employee> Items, int TotalCount)> GetPagedAsync(EmployeeListQueryDto query);
    Task<List<Employee>> GetRecentAsync(int count);
    Task<int> GetTotalCountAsync();
    Task<int> GetActiveCountAsync();
    Task<int> GetInactiveCountAsync();
    Task<List<(string Dept, int Count)>> GetDepartmentCountsAsync();
    Task<bool> EmailExistsAsync(string email, int? excludeId);
    Task<Employee> AddAsync(Employee employee);
    Task<Employee> UpdateAsync(Employee employee);
    Task<bool> DeleteAsync(int id);
}
