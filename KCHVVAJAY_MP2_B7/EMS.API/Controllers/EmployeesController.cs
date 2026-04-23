using EMS.API.DTOs;
using EMS.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers;

/// <summary>
/// Manages all Employee CRUD operations, the dashboard endpoint, and server-side
/// search/filter/sort/pagination. All endpoints require a valid JWT Bearer token.
/// Write operations (POST, PUT, DELETE) are restricted to the Admin role.
/// </summary>
[ApiController]
[Route("api/employees")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    /// <summary>Injects the employee service via constructor injection.</summary>
    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>
    /// GET /api/employees/dashboard
    /// Returns all KPI metrics (total, active, inactive, departments),
    /// department breakdown with percentages, and the last 5 employees added.
    /// Computed via SQL COUNT() and GROUP BY — no client-side aggregation.
    /// Accessible by both Admin and Viewer roles.
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard()
    {
        var dashboard = await _employeeService.GetDashboardAsync();
        return Ok(dashboard);
    }

    /// <summary>
    /// GET /api/employees?search=&amp;department=&amp;status=&amp;sortField=id&amp;sortDir=asc&amp;page=1&amp;pageSize=15
    /// Returns a paginated, filtered, and sorted list of employees.
    /// All filtering and sorting executes in SQL — no JavaScript processing.
    /// Search uses SQL LIKE on (FirstName + ' ' + LastName) and Email.
    /// Accessible by both Admin and Viewer roles.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<EmployeeResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] EmployeeListQueryDto query)
    {
        var result = await _employeeService.GetPagedAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// GET /api/employees/{id}
    /// Returns a single employee record by primary key.
    /// Returns 404 Not Found if the employee does not exist.
    /// Accessible by both Admin and Viewer roles.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var emp = await _employeeService.GetByIdAsync(id);
        if (emp == null)
            return NotFound(new { message = $"Employee with ID {id} not found." });

        return Ok(emp);
    }

    /// <summary>
    /// POST /api/employees
    /// Creates a new employee record. Admin role required.
    /// Validates all fields via Data Annotations on EmployeeRequestDto.
    /// Returns 201 Created with the new employee object on success.
    /// Returns 409 Conflict if the email address already belongs to another employee.
    /// Returns 403 Forbidden if called with a Viewer JWT.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] EmployeeRequestDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (employee, error) = await _employeeService.AddAsync(dto);

        if (error == "email_conflict")
            return Conflict(new { message = "An employee with this email already exists." });

        return CreatedAtAction(nameof(GetById), new { id = employee!.Id }, employee);
    }

    /// <summary>
    /// PUT /api/employees/{id}
    /// Updates an existing employee record. Admin role required.
    /// Allows the same email to be resubmitted (the current employee is excluded
    /// from the uniqueness check so edit-without-email-change works correctly).
    /// Returns 200 OK with the updated employee object on success.
    /// Returns 404 if the employee does not exist.
    /// Returns 409 Conflict if the email belongs to a different employee.
    /// Returns 403 Forbidden if called with a Viewer JWT.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] EmployeeRequestDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (employee, error) = await _employeeService.UpdateAsync(id, dto);

        return error switch
        {
            "not_found"      => NotFound(new { message = $"Employee with ID {id} not found." }),
            "email_conflict" => Conflict(new { message = "An employee with this email already exists." }),
            _                => Ok(employee)
        };
    }

    /// <summary>
    /// DELETE /api/employees/{id}
    /// Permanently removes an employee record by primary key. Admin role required.
    /// Returns 200 OK with a confirmation message on success.
    /// Returns 404 Not Found if the employee does not exist.
    /// Returns 403 Forbidden if called with a Viewer JWT.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _employeeService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Employee with ID {id} not found." });

        return Ok(new { message = $"Employee {id} deleted successfully." });
    }
}
