// app.js — Mini Project 2
// UPDATED from Mini Project 1:
//   • All service calls are async/await (storageService now makes HTTP calls)
//   • _state includes page + pageSize for server-side pagination
//   • Search debounced to 350ms before sending to API
//   • uiService.applyRoleUI() called after login
//   • uiService.renderPagination() called after every list render

$(document).ready(function () {

  // ── State ──────────────────────────────────────────────────────────────────
  let _state = {
    search:       '',
    filterDept:   'all',
    filterStatus: 'all',
    sortField:    'id',
    sortDir:      'asc',
    page:         1,
    pageSize:     15
  };

  let _searchDebounceTimer = null;

  // ── Spinner helpers ────────────────────────────────────────────────────────
  const showSpinner = () => $('#loading-spinner').css('display','flex');
  const hideSpinner = () => $('#loading-spinner').hide();

  // ── Init ───────────────────────────────────────────────────────────────────
  // Token is in-memory only — page refresh always shows login view
  const init = () => {
    uiService.showView('login');
  };

  // ── Helpers ────────────────────────────────────────────────────────────────

  const showSection = (section) => {
    uiService.showSection(section);
    if (section === 'dashboard') loadDashboard();
    if (section === 'employees') loadEmployeeList();
  };

  const loadDashboard = async () => {
    showSpinner();
    try {
      const summary   = await dashboardService.getSummary();
      const breakdown = await dashboardService.getDepartmentBreakdown();
      const recent    = await dashboardService.getRecentEmployees(5);
      uiService.renderDashboardCards(summary);
      uiService.renderDepartmentBreakdown(breakdown);
      uiService.renderRecentEmployees(recent);
    } catch (err) {
      uiService.showToast('Failed to load dashboard: ' + err.message, 'danger');
    } finally {
      hideSpinner();
    }
  };

  const loadEmployeeList = async () => {
    uiService.populateDeptFilter(employeeService.getDepartments());
    uiService.applyRoleUI();   // Re-enforce role UI on every section load
    await applyAndRender();
  };

  const applyAndRender = async () => {
    showSpinner();
    try {
      const result = await employeeService.getAll({
        search:     _state.search,
        department: _state.filterDept,
        status:     _state.filterStatus,
        sortField:  _state.sortField,
        sortDir:    _state.sortDir,
        page:       _state.page,
        pageSize:   _state.pageSize
      });
      // result = { data[], totalCount, page, pageSize, totalPages }
      uiService.renderEmployeeTable(result.data);
      uiService.renderPagination(result.page, result.totalPages, result.totalCount, result.pageSize);
      updateSortIndicators();
    } catch (err) {
      uiService.showToast('Failed to load employees: ' + err.message, 'danger');
    } finally {
      hideSpinner();
    }
  };

  const updateSortIndicators = () => {
    $('.sort-icon').text('↕');
    if (_state.sortField) {
      const icon = _state.sortDir === 'asc' ? '▲' : '▼';
      $(`[data-sort="${_state.sortField}"] .sort-icon`).text(icon);
    }
  };

  const refreshAll = async () => {
    await loadDashboard();
    await applyAndRender();
  };

  // ── Pagination click ───────────────────────────────────────────────────────

  $(document).on('click', '.page-link', function (e) {
    e.preventDefault();
    const $li = $(this).closest('.page-item');
    if ($li.hasClass('disabled') || $li.hasClass('active')) return;
    const newPage = parseInt($(this).data('page'), 10);
    if (!isNaN(newPage)) {
      _state.page = newPage;
      applyAndRender();
    }
  });

  // ── Navigation ─────────────────────────────────────────────────────────────

  $('#nav-dashboard').on('click', function (e) {
    e.preventDefault();
    showSection('dashboard');
  });

  $('#nav-employees').on('click', function (e) {
    e.preventDefault();
    showSection('employees');
  });

  $('#btn-add-employee-nav, #btn-add-employee-page').on('click', function () {
    showSection('employees');
    uiService.showAddEditModal(null);
  });

  $('#btn-logout').on('click', function () {
    authService.logout();
    uiService.showView('login');
    uiService.clearAuthError('login-error');
  });

  // ── Signup ─────────────────────────────────────────────────────────────────

  $('#btn-go-signup').on('click', function () {
    uiService.showView('signup');
    uiService.clearAuthError('signup-error');
  });

  $('#btn-go-login').on('click', function () {
    uiService.showView('login');
    uiService.clearAuthError('login-error');
  });

  $('#form-signup').on('submit', async function (e) {
    e.preventDefault();
    const formData = {
      username:        $('#signup-username').val().trim(),
      password:        $('#signup-password').val(),
      confirmPassword: $('#signup-confirm').val()
    };
    uiService.clearAuthError('signup-error');
    $('#signup-username, #signup-password, #signup-confirm').removeClass('is-invalid');
    $('.signup-field-error').text('').hide();

    const errors = validationService.validateAuthForm(formData, true);
    if (Object.keys(errors).length > 0) {
      if (errors.username)        { $('#signup-username').addClass('is-invalid'); $('#err-signup-username').text(errors.username).show(); }
      if (errors.password)        { $('#signup-password').addClass('is-invalid'); $('#err-signup-password').text(errors.password).show(); }
      if (errors.confirmPassword) { $('#signup-confirm').addClass('is-invalid');  $('#err-signup-confirm').text(errors.confirmPassword).show(); }
      return;
    }

    const result = await authService.signup(formData.username, formData.password, 'Admin');
    if (!result.success) {
      if (result.error === 'username_taken') {
        $('#signup-username').addClass('is-invalid');
        $('#err-signup-username').text('Username is already taken. Please choose another.').show();
      } else {
        uiService.showAuthError(result.message || 'Registration failed.', 'signup-error');
      }
      return;
    }
    uiService.showToast('Account created successfully! Please login.', 'success');
    setTimeout(() => uiService.showView('login'), 1500);
  });

  // ── Login ──────────────────────────────────────────────────────────────────

  $('#form-login').on('submit', async function (e) {
    e.preventDefault();
    const formData = {
      username: $('#login-username').val().trim(),
      password: $('#login-password').val()
    };
    uiService.clearAuthError('login-error');
    $('#login-username, #login-password').removeClass('is-invalid');
    $('.login-field-error').text('').hide();

    const errors = validationService.validateAuthForm(formData, false);
    if (Object.keys(errors).length > 0) {
      if (errors.username) { $('#login-username').addClass('is-invalid'); $('#err-login-username').text(errors.username).show(); }
      if (errors.password) { $('#login-password').addClass('is-invalid'); $('#err-login-password').text(errors.password).show(); }
      return;
    }

    const result = await authService.login(formData.username, formData.password);
    if (!result.success) {
      uiService.showAuthError('Invalid username or password.', 'login-error');
      return;
    }

    $('#nav-current-user').text(authService.getCurrentUser());
    uiService.showView('app');
    uiService.applyRoleUI();        // NEW in MP2: role-based show/hide
    loadDashboard();
    showSection('dashboard');
  });

  // ── Search (debounced 350ms) ───────────────────────────────────────────────

  $('#search-input').on('input', function () {
    clearTimeout(_searchDebounceTimer);
    _searchDebounceTimer = setTimeout(() => {
      _state.search = $(this).val();
      _state.page   = 1;
      applyAndRender();
    }, 350);
  });

  // ── Department Filter ──────────────────────────────────────────────────────

  $('#filter-department').on('change', function () {
    _state.filterDept = $(this).val();
    _state.page       = 1;
    applyAndRender();
  });

  // ── Status Filter ──────────────────────────────────────────────────────────

  $(document).on('click', '.btn-status-filter', function () {
    $('.btn-status-filter').removeClass('active');
    $(this).addClass('active');
    _state.filterStatus = $(this).data('status');
    _state.page         = 1;
    applyAndRender();
  });

  // ── Sorting ────────────────────────────────────────────────────────────────

  $(document).on('click', '[data-sort]', function () {
    const field = $(this).data('sort');
    if (_state.sortField === field) {
      _state.sortDir = _state.sortDir === 'asc' ? 'desc' : 'asc';
    } else {
      _state.sortField = field;
      _state.sortDir   = 'asc';
    }
    _state.page = 1;
    applyAndRender();
  });

  // ── Add / Edit Employee ────────────────────────────────────────────────────

  $(document).on('click', '#btn-save-employee', async function () {
    const formData = uiService.getFormData();
    const mode     = $(this).data('mode');
    const editId   = mode === 'edit' ? $(this).data('id') : null;

    uiService.clearInlineErrors();
    const errors = validationService.validateEmployeeForm(formData, editId);
    if (Object.keys(errors).length > 0) {
      uiService.showInlineErrors(errors);
      return;
    }

    try {
      if (mode === 'edit') {
        await employeeService.update(editId, formData);
        uiService.showToast('Employee updated successfully.', 'success');
      } else {
        await employeeService.add(formData);
        uiService.showToast('Employee added successfully.', 'success');
      }
      bootstrap.Modal.getInstance(document.getElementById('modal-add-edit'))?.hide();
      _state.page = 1;
      await refreshAll();
    } catch (err) {
      if (err.status === 409) {
        uiService.showInlineErrors({ email: 'This email already exists.' });
      } else if (err.status === 400 && err.data?.errors) {
        const serverErrors = {};
        Object.entries(err.data.errors).forEach(([key, msgs]) => {
          const fieldKey = key.charAt(0).toLowerCase() + key.slice(1);
          serverErrors[fieldKey] = Array.isArray(msgs) ? msgs[0] : msgs;
        });
        uiService.showInlineErrors(serverErrors);
      } else {
        uiService.showToast('Error: ' + err.message, 'danger');
      }
    }
  });

  // ── View Employee ──────────────────────────────────────────────────────────

  $(document).on('click', '.btn-action-view', async function () {
    const id = Number($(this).data('id'));
    try {
      const emp = await employeeService.getById(id);
      if (emp) uiService.showViewModal(emp);
    } catch (err) {
      uiService.showToast('Could not load employee: ' + err.message, 'danger');
    }
  });

  // ── Edit Employee ──────────────────────────────────────────────────────────

  $(document).on('click', '.btn-action-edit', async function () {
    const id = Number($(this).data('id'));
    try {
      const emp = await employeeService.getById(id);
      if (emp) uiService.showAddEditModal(emp);
    } catch (err) {
      uiService.showToast('Could not load employee: ' + err.message, 'danger');
    }
  });

  // ── Delete Employee ────────────────────────────────────────────────────────

  $(document).on('click', '.btn-action-delete', async function () {
    const id = Number($(this).data('id'));
    try {
      const emp = await employeeService.getById(id);
      if (emp) uiService.showDeleteModal(emp);
    } catch (err) {
      uiService.showToast('Could not load employee: ' + err.message, 'danger');
    }
  });

  $(document).on('click', '#btn-confirm-delete', async function () {
    const id = Number($(this).data('id'));
    try {
      await employeeService.remove(id);
      bootstrap.Modal.getInstance(document.getElementById('modal-delete'))?.hide();
      uiService.showToast('Employee deleted successfully.', 'danger');
      _state.page = 1;
      await refreshAll();
    } catch (err) {
      uiService.showToast('Delete failed: ' + err.message, 'danger');
    }
  });

  // ── Dashboard Add Employee button ──────────────────────────────────────────

  $(document).on('click', '#btn-add-emp-dashboard', function () {
    showSection('employees');
    uiService.showAddEditModal(null);
  });

  // ── Initialise ─────────────────────────────────────────────────────────────
  init();

});
