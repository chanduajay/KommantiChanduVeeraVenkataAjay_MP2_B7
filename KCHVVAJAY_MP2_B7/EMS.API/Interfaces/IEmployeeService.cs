using EMS.API.DTOs;

namespace EMS.API.Interfaces;

public interface IEmployeeService
{
    Task<EmployeeResponseDto?> GetByIdAsync(int id);
    Task<PagedResultDto<EmployeeResponseDto>> GetPagedAsync(EmployeeListQueryDto query);
    Task<DashboardDto> GetDashboardAsync();
    Task<(EmployeeResponseDto? Employee, string? Error)> AddAsync(EmployeeRequestDto dto);
    Task<(EmployeeResponseDto? Employee, string? Error)> UpdateAsync(int id, EmployeeRequestDto dto);
    Task<bool> DeleteAsync(int id);
}
