using EMS.API.DTOs;
using EMS.API.Interfaces;
using EMS.API.Models;
using EMS.API.Services;
using Moq;
using NUnit.Framework;

namespace EMS.Tests;

[TestFixture]
public class EmployeeServiceTests
{
    private Mock<IEmployeeRepository> _repoMock = null!;
    private EmployeeService           _service  = null!;

    [SetUp]
    public void Setup()
    {
        _repoMock = new Mock<IEmployeeRepository>();
        _service  = new EmployeeService(_repoMock.Object);
    }

    // ── GetByIdAsync ──────────────────────────────────────────────────────────

    [Test]
    public async Task GetByIdAsync_ValidId_ReturnsMappedDto()
    {
        // Arrange — tell the mock what to return
        var fakeEmployee = new Employee
        {
            Id          = 1,
            FirstName   = "Priya",
            LastName    = "Prabhu",
            Email       = "priya.prabhu@gmail.com",
            Phone       = "9876543210",
            Department  = "Engineering",
            Designation = "Software Engineer",
            Salary      = 800000,
            JoinDate    = new DateTime(2021, 3, 15),
            Status      = "Active",
            CreatedAt   = DateTime.UtcNow
        };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeEmployee);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.That(result,            Is.Not.Null);
        Assert.That(result!.FirstName, Is.EqualTo("Priya"));
        Assert.That(result.LastName,   Is.EqualTo("Prabhu"));
        Assert.That(result.Id,         Is.EqualTo(1));
        _repoMock.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(9999)).ReturnsAsync((Employee?)null);

        var result = await _service.GetByIdAsync(9999);

        Assert.That(result, Is.Null);
    }

    // ── AddAsync ──────────────────────────────────────────────────────────────

    [Test]
    public async Task AddAsync_UniqueEmail_ReturnsNewEmployee()
    {
        var dto = new EmployeeRequestDto
        {
            FirstName   = "Ravi",
            LastName    = "Kumar",
            Email       = "ravi.kumar@gmail.com",
            Phone       = "9876543211",
            Department  = "Finance",
            Designation = "Analyst",
            Salary      = 600000,
            JoinDate    = new DateTime(2023, 1, 1),
            Status      = "Active"
        };

        _repoMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), null))
                 .ReturnsAsync(false);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Employee>()))
                 .ReturnsAsync((Employee e) => { e.Id = 99; return e; });

        var (employee, error) = await _service.AddAsync(dto);

        Assert.That(error,           Is.Null);
        Assert.That(employee,        Is.Not.Null);
        Assert.That(employee!.Email, Is.EqualTo("ravi.kumar@gmail.com"));
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Once);
    }

    [Test]
    public async Task AddAsync_DuplicateEmail_ReturnsEmailConflictError()
    {
        var dto = new EmployeeRequestDto
        {
            FirstName   = "Test",
            LastName    = "User",
            Email       = "existing@gmail.com",
            Phone       = "9876543212",
            Department  = "HR",
            Designation = "Executive",
            Salary      = 500000,
            JoinDate    = DateTime.Now,
            Status      = "Active"
        };

        _repoMock.Setup(r => r.EmailExistsAsync("existing@gmail.com", null))
                 .ReturnsAsync(true);

        var (employee, error) = await _service.AddAsync(dto);

        Assert.That(employee, Is.Null);
        Assert.That(error,    Is.EqualTo("email_conflict"));
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Never);
    }

    // ── UpdateAsync ───────────────────────────────────────────────────────────

    [Test]
    public async Task UpdateAsync_ValidId_ReturnsUpdatedEmployee()
    {
        var existing = new Employee
        {
            Id          = 1,
            FirstName   = "Old",
            LastName    = "Name",
            Email       = "old@gmail.com",
            Phone       = "9876543210",
            Department  = "HR",
            Designation = "Executive",
            Salary      = 500000,
            JoinDate    = DateTime.Now,
            Status      = "Active"
        };
        var dto = new EmployeeRequestDto
        {
            FirstName   = "New",
            LastName    = "Name",
            Email       = "old@gmail.com",
            Phone       = "9876543210",
            Department  = "Engineering",
            Designation = "Engineer",
            Salary      = 700000,
            JoinDate    = DateTime.Now,
            Status      = "Active"
        };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _repoMock.Setup(r => r.EmailExistsAsync("old@gmail.com", 1)).ReturnsAsync(false);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Employee>()))
                 .ReturnsAsync((Employee e) => e);

        var (employee, error) = await _service.UpdateAsync(1, dto);

        Assert.That(error,               Is.Null);
        Assert.That(employee,            Is.Not.Null);
        Assert.That(employee!.FirstName,  Is.EqualTo("New"));
        Assert.That(employee.Department,  Is.EqualTo("Engineering"));
    }

    [Test]
    public async Task UpdateAsync_NonExistentId_ReturnsNotFoundError()
    {
        _repoMock.Setup(r => r.GetByIdAsync(9999)).ReturnsAsync((Employee?)null);

        var (employee, error) = await _service.UpdateAsync(9999, new EmployeeRequestDto
        {
            FirstName   = "X",
            LastName    = "Y",
            Email       = "x@y.com",
            Phone       = "1234567890",
            Department  = "HR",
            Designation = "E",
            Salary      = 1,
            JoinDate    = DateTime.Now,
            Status      = "Active"
        });

        Assert.That(employee, Is.Null);
        Assert.That(error,    Is.EqualTo("not_found"));
    }

    // ── DeleteAsync ───────────────────────────────────────────────────────────

    [Test]
    public async Task DeleteAsync_ExistingId_ReturnsTrue()
    {
        _repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _service.DeleteAsync(1);

        Assert.That(result, Is.True);
        _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_NonExistentId_ReturnsFalse()
    {
        _repoMock.Setup(r => r.DeleteAsync(9999)).ReturnsAsync(false);

        var result = await _service.DeleteAsync(9999);

        Assert.That(result, Is.False);
    }

    // ── GetDashboardAsync ─────────────────────────────────────────────────────

    [Test]
    public async Task GetDashboardAsync_ReturnsCorrectSummary()
    {
        _repoMock.Setup(r => r.GetTotalCountAsync()).ReturnsAsync(15);
        _repoMock.Setup(r => r.GetActiveCountAsync()).ReturnsAsync(11);
        _repoMock.Setup(r => r.GetInactiveCountAsync()).ReturnsAsync(4);
        _repoMock.Setup(r => r.GetDepartmentCountsAsync())
                 .ReturnsAsync(new List<(string, int)>
                 {
                     ("Engineering", 5),
                     ("Finance",     3),
                     ("HR",          1),
                     ("Marketing",   3),
                     ("Operations",  3)
                 });
        _repoMock.Setup(r => r.GetRecentAsync(5))
                 .ReturnsAsync(new List<Employee>
                 {
                     new Employee
                     {
                         Id = 15, FirstName = "Amit", LastName = "Narayana",
                         Email = "amit.narayana@gmail.com", Phone = "9844567891",
                         Department = "Operations", Designation = "Supply Chain Analyst",
                         Salary = 650000, JoinDate = DateTime.Now, Status = "Inactive"
                     }
                 });

        var dashboard = await _service.GetDashboardAsync();

        Assert.That(dashboard.TotalEmployees,    Is.EqualTo(15));
        Assert.That(dashboard.ActiveEmployees,   Is.EqualTo(11));
        Assert.That(dashboard.InactiveEmployees, Is.EqualTo(4));
        Assert.That(dashboard.TotalDepartments,  Is.EqualTo(5));
        Assert.That(dashboard.RecentEmployees,   Has.Count.EqualTo(1));
    }
}
