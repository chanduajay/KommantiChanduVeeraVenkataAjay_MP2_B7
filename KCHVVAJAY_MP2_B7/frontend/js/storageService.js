// storageService.js — Mini Project 2
// REPLACED from Mini Project 1.
// All methods now make real HTTP fetch() calls to the .NET 8 Web API.
// This is the ONLY file that knows the API endpoint URLs.
// All other modules (employeeService, dashboardService, authService) call
// the same method names they used in Mini Project 1 — zero changes there.

const storageService = (() => {

  // ── Private: build headers, attach Bearer token when needed ──────────────
  const _headers = (withAuth = true) => {
    const headers = { 'Content-Type': 'application/json' };
    if (withAuth) {
      const token = (typeof authService !== 'undefined') ? authService.getToken() : null;
      if (token) headers['Authorization'] = `Bearer ${token}`;
    }
    return headers;
  };

  // ── Private: centralised fetch with error handling ────────────────────────
  const _fetch = async (url, options = {}) => {
    const response = await fetch(url, options);

    // 204 No Content — return null (no body to parse)
    if (response.status === 204) return null;

    const data = await response.json().catch(() => ({}));

    if (!response.ok) {
      const err   = new Error(data?.message || data?.title || `HTTP ${response.status}`);
      err.status  = response.status;
      err.data    = data;
      throw err;
    }
    return data;
  };

  // ── Auth ──────────────────────────────────────────────────────────────────

  // POST /api/auth/login → { success, token, username, role, message }
  const login = async ({ username, password }) =>
    await _fetch(`${API_BASE_URL}/auth/login`, {
      method:  'POST',
      headers: _headers(false),
      body:    JSON.stringify({ username, password })
    });

  // POST /api/auth/register → { success, username, role, message }
  const register = async ({ username, password, role }) =>
    await _fetch(`${API_BASE_URL}/auth/register`, {
      method:  'POST',
      headers: _headers(false),
      body:    JSON.stringify({ username, password, role })
    });

  // ── Dashboard ─────────────────────────────────────────────────────────────

  // GET /api/employees/dashboard → { totalEmployees, activeEmployees,
  //   inactiveEmployees, totalDepartments, departmentBreakdown[], recentEmployees[] }
  const getDashboard = async () =>
    await _fetch(`${API_BASE_URL}/employees/dashboard`, {
      method:  'GET',
      headers: _headers()
    });

  // ── Employee List (server-side search + filter + sort + pagination) ────────

  // GET /api/employees?search=&department=&status=&sortField=&sortDir=&page=&pageSize=
  // Returns: { data[], totalCount, page, pageSize, totalPages }
  const getAll = async (queryParams = {}) => {
    const params = new URLSearchParams();

    if (queryParams.search)
      params.append('search', queryParams.search);

    if (queryParams.department && queryParams.department !== 'all')
      params.append('department', queryParams.department);

    if (queryParams.status && queryParams.status !== 'all')
      params.append('status', queryParams.status);

    if (queryParams.sortField)
      params.append('sortField', queryParams.sortField);

    if (queryParams.sortDir)
      params.append('sortDir', queryParams.sortDir);

    if (queryParams.page)
      params.append('page', queryParams.page);

    if (queryParams.pageSize)
      params.append('pageSize', queryParams.pageSize);

    const qs = params.toString();
    return await _fetch(`${API_BASE_URL}/employees${qs ? '?' + qs : ''}`, {
      method:  'GET',
      headers: _headers()
    });
  };

  // GET /api/employees/{id}
  const getById = async (id) =>
    await _fetch(`${API_BASE_URL}/employees/${id}`, {
      method:  'GET',
      headers: _headers()
    });

  // POST /api/employees → 201 Created
  const add = async (employeeDto) =>
    await _fetch(`${API_BASE_URL}/employees`, {
      method:  'POST',
      headers: _headers(),
      body:    JSON.stringify(employeeDto)
    });

  // PUT /api/employees/{id} → 200 OK
  const update = async (id, employeeDto) =>
    await _fetch(`${API_BASE_URL}/employees/${id}`, {
      method:  'PUT',
      headers: _headers(),
      body:    JSON.stringify(employeeDto)
    });

  // DELETE /api/employees/{id} → 200 OK
  const remove = async (id) =>
    await _fetch(`${API_BASE_URL}/employees/${id}`, {
      method:  'DELETE',
      headers: _headers()
    });

  return { login, register, getDashboard, getAll, getById, add, update, remove };

})();
