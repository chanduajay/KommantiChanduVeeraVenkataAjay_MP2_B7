// uiService.js — Mini Project 2
// All Mini Project 1 rendering is preserved exactly.
// New additions for MP2:
//   applyRoleUI()    — shows/hides Add/Edit/Delete buttons based on role
//   renderPagination() — Bootstrap pagination bar below the employee table

const uiService = (() => {

  // ── Helpers ───────────────────────────────────────────────────────────────

  const formatSalary = (amount) => {
    const num = Number(amount);
    return '₹' + num.toLocaleString('en-IN');
  };

  const formatDate = (dateStr) => {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    return d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
  };

  const getInitials = (firstName, lastName) => {
    return ((firstName || '')[0] || '').toUpperCase() +
           ((lastName  || '')[0] || '').toUpperCase();
  };

  const deptColorClass = (dept) => {
    const map = {
      'Engineering': 'badge-engineering',
      'Marketing':   'badge-marketing',
      'HR':          'badge-hr',
      'Finance':     'badge-finance',
      'Operations':  'badge-operations'
    };
    return map[dept] || 'bg-secondary';
  };

  const avatarColorClass = (name) => {
    const colors = ['avatar-blue','avatar-green','avatar-purple',
                    'avatar-orange','avatar-teal','avatar-red','avatar-indigo'];
    let hash = 0;
    for (let i = 0; i < name.length; i++)
      hash = name.charCodeAt(i) + ((hash << 5) - hash);
    return colors[Math.abs(hash) % colors.length];
  };

  // ── Views ─────────────────────────────────────────────────────────────────

  const showView = (viewName) => {
    $('#view-signup, #view-login').css('display', 'none');
    $('#view-app').hide();
    if (viewName === 'app') {
      $('#view-app').show();
      $('#main-navbar').removeClass('d-none');
    } else {
      $('#main-navbar').addClass('d-none');
      $(`#view-${viewName}`).css('display', 'flex');
    }
  };

  const showSection = (section) => {
    $('#section-dashboard, #section-employees').hide();
    $(`#section-${section}`).show();
    $('#nav-dashboard, #nav-employees').removeClass('active');
    $(`#nav-${section}`).addClass('active');
  };

  // ── NEW in MP2: Role-Based UI ──────────────────────────────────────────────
  // Called once after every login. Shows/hides elements based on role.

  const applyRoleUI = () => {
    const isAdm = authService.isAdmin();
    const role  = authService.getRole() || 'Viewer';

    // Update role badge in navbar
    $('#nav-role-badge')
      .text(role)
      .removeClass('badge-role-admin badge-role-viewer')
      .addClass(isAdm ? 'badge-role-admin' : 'badge-role-viewer');

    // Add Employee button: visible to Admin only
    if (isAdm) {
      $('#btn-add-employee-nav, #btn-add-employee-page, #btn-add-emp-dashboard').show();
    } else {
      $('#btn-add-employee-nav, #btn-add-employee-page, #btn-add-emp-dashboard').hide();
    }

    // Viewer notice bar in Employee List
    if (isAdm) {
      $('#viewer-notice').hide();
    } else {
      $('#viewer-notice').show();
    }
  };

  // Called after renderEmployeeTable() to enforce row-level role restrictions
  const _applyRowRoleUI = () => {
    if (!authService.isAdmin()) {
      $('.btn-action-edit, .btn-action-delete').hide();
    }
  };

  // ── Dashboard ─────────────────────────────────────────────────────────────

  const renderDashboardCards = (summary) => {
    $('#kpi-total').text(summary.total);
    $('#kpi-active').text(summary.active);
    $('#kpi-inactive').text(summary.inactive);
    $('#kpi-departments').text(summary.departments);
  };

  const renderDepartmentBreakdown = (breakdown) => {
    const total  = breakdown.reduce((s, d) => s + d.count, 0);
    const $tbody = $('#dept-breakdown-body').empty();
    if (!breakdown.length) {
      $tbody.append('<tr><td colspan="3" class="text-center text-muted">No data</td></tr>');
      return;
    }
    breakdown.forEach(({ dept, count, percentage }) => {
      const pct = percentage ?? (total > 0 ? Math.round((count / total) * 100) : 0);
      $tbody.append(`
        <tr>
          <td><span class="dept-badge ${deptColorClass(dept)}">${dept}</span></td>
          <td class="text-center fw-semibold">${count}</td>
          <td>
            <div class="dept-bar-wrap">
              <div class="dept-bar ${deptColorClass(dept)}" style="width:${pct}%"></div>
            </div>
            <small class="text-muted">${pct}%</small>
          </td>
        </tr>`);
    });
  };

  const renderRecentEmployees = (employees) => {
    const $list = $('#recent-employees-list').empty();
    if (!employees.length) {
      $list.append('<div class="text-muted text-center p-3">No employees yet.</div>');
      return;
    }
    employees.forEach(e => {
      const initials = getInitials(e.firstName, e.lastName);
      const colorCls = avatarColorClass(initials);
      $list.append(`
        <div class="recent-emp-item d-flex align-items-center gap-3 p-2">
          <div class="emp-avatar ${colorCls}">${initials}</div>
          <div class="flex-grow-1">
            <div class="fw-semibold">${e.firstName} ${e.lastName}</div>
            <div class="text-muted small">${e.designation}</div>
          </div>
          <div class="d-flex flex-column align-items-end gap-1">
            <span class="dept-badge ${deptColorClass(e.department)}">${e.department}</span>
            <span class="status-badge ${e.status === 'Active' ? 'status-active' : 'status-inactive'}">${e.status}</span>
          </div>
        </div>`);
    });
  };

  // ── Employee Table ─────────────────────────────────────────────────────────

  const renderEmployeeTable = (employees) => {
    const $tbody = $('#employee-table-body').empty();
    if (!employees.length) {
      $tbody.append('<tr><td colspan="10" class="text-center text-muted py-4"><i class="bi bi-search me-2"></i>No employees found.</td></tr>');
      return;
    }
    employees.forEach((e, idx) => {
      const initials = getInitials(e.firstName, e.lastName);
      const colorCls = avatarColorClass(initials);
      $tbody.append(`
        <tr class="${idx % 2 === 1 ? 'row-alt' : ''}">
          <td class="text-muted">#${e.id}</td>
          <td><div class="emp-avatar emp-avatar-sm ${colorCls}">${initials}</div></td>
          <td><span class="fw-semibold">${e.firstName} ${e.lastName}</span></td>
          <td class="text-muted small">${e.email}</td>
          <td><span class="dept-badge ${deptColorClass(e.department)}">${e.department}</span></td>
          <td>${e.designation}</td>
          <td class="fw-semibold">${formatSalary(e.salary)}</td>
          <td>${formatDate(e.joinDate)}</td>
          <td><span class="status-badge ${e.status === 'Active' ? 'status-active' : 'status-inactive'}">${e.status}</span></td>
          <td>
            <div class="d-flex gap-1">
              <button class="btn btn-sm btn-action-view"   data-id="${e.id}" title="View"><i class="bi bi-eye"></i></button>
              <button class="btn btn-sm btn-action-edit"   data-id="${e.id}" title="Edit"><i class="bi bi-pencil"></i></button>
              <button class="btn btn-sm btn-action-delete" data-id="${e.id}" title="Delete"><i class="bi bi-trash"></i></button>
            </div>
          </td>
        </tr>`);
    });
    // Apply role restrictions to newly rendered rows
    _applyRowRoleUI();
  };

  const updateTotalCount = (total) => {
    $('#employee-count-total').data('total', total);
  };

  // ── NEW in MP2: Pagination Bar ─────────────────────────────────────────────
  // Renders Bootstrap pagination below the employee table.
  // Shows "Showing X–Y of Z employees" label.

  const renderPagination = (page, totalPages, totalCount, pageSize) => {
    const start = totalCount === 0 ? 0 : (page - 1) * pageSize + 1;
    const end   = Math.min(page * pageSize, totalCount);
    $('#employee-count-label').text(`Showing ${start}–${end} of ${totalCount} employees`);

    const $pag = $('#pagination-container').empty();
    if (totalPages <= 1) return;

    const $ul = $('<ul class="pagination pagination-sm mb-0"></ul>');

    // Prev button
    $ul.append(`
      <li class="page-item ${page <= 1 ? 'disabled' : ''}">
        <a class="page-link" href="#" data-page="${page - 1}">&#8249; Prev</a>
      </li>`);

    // Page numbers — show up to 5 around current page
    const startPage = Math.max(1, page - 2);
    const endPage   = Math.min(totalPages, page + 2);

    if (startPage > 1) {
      $ul.append(`<li class="page-item"><a class="page-link" href="#" data-page="1">1</a></li>`);
      if (startPage > 2)
        $ul.append(`<li class="page-item disabled"><span class="page-link">…</span></li>`);
    }

    for (let p = startPage; p <= endPage; p++) {
      $ul.append(`
        <li class="page-item ${p === page ? 'active' : ''}">
          <a class="page-link" href="#" data-page="${p}">${p}</a>
        </li>`);
    }

    if (endPage < totalPages) {
      if (endPage < totalPages - 1)
        $ul.append(`<li class="page-item disabled"><span class="page-link">…</span></li>`);
      $ul.append(`<li class="page-item"><a class="page-link" href="#" data-page="${totalPages}">${totalPages}</a></li>`);
    }

    // Next button
    $ul.append(`
      <li class="page-item ${page >= totalPages ? 'disabled' : ''}">
        <a class="page-link" href="#" data-page="${page + 1}">Next &#8250;</a>
      </li>`);

    $pag.append($ul);
  };

  // ── Department Filter Dropdown ─────────────────────────────────────────────

  const populateDeptFilter = (departments) => {
    const $sel    = $('#filter-department');
    const current = $sel.val();
    $sel.empty().append('<option value="all">All Departments</option>');
    departments.forEach(d => $sel.append(`<option value="${d}">${d}</option>`));
    if (current) $sel.val(current);
  };

  // ── Modals ─────────────────────────────────────────────────────────────────

  const showAddEditModal = (employee = null) => {
    clearForm();
    clearInlineErrors();
    if (employee) {
      $('#modal-emp-title').text('Edit Employee');
      $('#btn-save-employee').text('Update Employee').data('mode', 'edit').data('id', employee.id);
      populateForm(employee);
    } else {
      $('#modal-emp-title').text('Add Employee');
      $('#btn-save-employee').text('Save Employee').data('mode', 'add').removeData('id');
    }
    bootstrap.Modal.getOrCreateInstance(document.getElementById('modal-add-edit')).show();
  };

  const populateForm = (employee) => {
    $('#inp-firstName').val(employee.firstName);
    $('#inp-lastName').val(employee.lastName);
    $('#inp-email').val(employee.email);
    $('#inp-phone').val(employee.phone);
    $('#inp-department').val(employee.department);
    $('#inp-designation').val(employee.designation);
    $('#inp-salary').val(employee.salary);
    $('#inp-joinDate').val(employee.joinDate);   // "yyyy-MM-dd" from API
    $('#inp-status').val(employee.status);
  };

  const clearForm = () => { $('#form-add-edit')[0].reset(); };

    const showViewModal = (employee) => {
        const initials = getInitials(employee.firstName, employee.lastName);
        const colorCls = avatarColorClass(initials);
        // Large avatar centred above name — uses view-modal-avatar class (90px)
        $('#view-modal-avatar').attr('class', `emp-avatar view-modal-avatar ${colorCls}`).text(initials);
        $('#view-modal-name').text(`${employee.firstName} ${employee.lastName}`);
        // Department badge — solid filled pill under name
        $('#view-modal-dept').html(
            `<span class="dept-badge ${deptColorClass(employee.department)}">${employee.department}</span>`);
        $('#view-modal-email').text(employee.email);
        $('#view-modal-phone').text(employee.phone);
        $('#view-modal-designation').text(employee.designation);
        $('#view-modal-salary').text(formatSalary(employee.salary));
        $('#view-modal-joinDate').text(formatDate(employee.joinDate));
        // Solid filled status pill — matches reference
        $('#view-modal-status').html(
            `<span class="${employee.status === 'Active' ? 'status-active' : 'status-inactive'}">${employee.status}</span>`);
        bootstrap.Modal.getOrCreateInstance(document.getElementById('modal-view')).show();
    };
  const showDeleteModal = (employee) => {
    $('#delete-modal-name').text(`${employee.firstName} ${employee.lastName}`);
    $('#btn-confirm-delete').data('id', employee.id);
    bootstrap.Modal.getOrCreateInstance(document.getElementById('modal-delete')).show();
  };

  // ── Inline Errors ──────────────────────────────────────────────────────────

  const showInlineErrors = (errors) => {
    clearInlineErrors();
    Object.entries(errors).forEach(([field, msg]) => {
      $(`#inp-${field}`).addClass('is-invalid');
      $(`#err-${field}`).text(msg).show();
    });
  };

  const clearInlineErrors = () => {
    $('.is-invalid').removeClass('is-invalid');
    $('.field-error').text('').hide();
  };

  const showAuthError  = (msg, targetId) => $(`#${targetId}`).text(msg).show();
  const clearAuthError = (targetId)       => $(`#${targetId}`).text('').hide();

  // ── Toast ──────────────────────────────────────────────────────────────────

  const showToast = (message, type = 'success') => {
    const icons = {
      success: 'bi-check-circle-fill',
      danger:  'bi-x-circle-fill',
      warning: 'bi-exclamation-circle-fill'
    };
    const icon   = icons[type] || icons.success;
    const $toast = $(`
      <div class="toast align-items-center text-bg-${type} border-0 mb-2"
           role="alert" aria-live="assertive" aria-atomic="true">
        <div class="d-flex">
          <div class="toast-body"><i class="bi ${icon} me-2"></i>${message}</div>
          <button type="button" class="btn-close btn-close-white me-2 m-auto"
                  data-bs-dismiss="toast"></button>
        </div>
      </div>`);
    $('#toast-container').append($toast);
    const t = new bootstrap.Toast($toast[0], { delay: 3000 });
    t.show();
    $toast[0].addEventListener('hidden.bs.toast', () => $toast.remove());
  };

  const getFormData = () => ({
    firstName:   $('#inp-firstName').val().trim(),
    lastName:    $('#inp-lastName').val().trim(),
    email:       $('#inp-email').val().trim(),
    phone:       $('#inp-phone').val().trim(),
    department:  $('#inp-department').val(),
    designation: $('#inp-designation').val().trim(),
    salary:      $('#inp-salary').val().trim() === '' ? '' : Number($('#inp-salary').val()),
    joinDate:    $('#inp-joinDate').val(),
    status:      $('#inp-status').val()
  });

  return {
    showView, showSection,
    applyRoleUI,
    renderDashboardCards, renderDepartmentBreakdown, renderRecentEmployees,
    renderEmployeeTable, updateTotalCount, populateDeptFilter,
    renderPagination,
    showAddEditModal, showViewModal, showDeleteModal,
    showInlineErrors, clearInlineErrors, showAuthError, clearAuthError,
    showToast, getFormData, formatSalary, formatDate
  };

})();
