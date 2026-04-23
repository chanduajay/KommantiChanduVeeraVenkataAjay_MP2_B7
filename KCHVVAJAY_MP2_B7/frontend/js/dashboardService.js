// dashboardService.js — Mini Project 2
// Architecture UNCHANGED from Mini Project 1.
// Same three method names. Now calls storageService.getDashboard() (one API call)
// instead of computing from the in-memory array.

const dashboardService = (() => {

  // All four KPI values + breakdown + recent employees in a single API call
  const getSummary = async () => {
    const data = await storageService.getDashboard();
    return {
      total:       data.totalEmployees,
      active:      data.activeEmployees,
      inactive:    data.inactiveEmployees,
      departments: data.totalDepartments
    };
  };

  // Returns array of { dept, count, percentage } — sorted alphabetically by API
  const getDepartmentBreakdown = async () => {
    const data = await storageService.getDashboard();
    return data.departmentBreakdown;
  };

  // Returns last 5 employees added, ordered by CreatedAt desc then Id desc
  const getRecentEmployees = async (n = 5) => {
    const data = await storageService.getDashboard();
    return data.recentEmployees.slice(0, n);
  };

  return { getSummary, getDepartmentBreakdown, getRecentEmployees };

})();
