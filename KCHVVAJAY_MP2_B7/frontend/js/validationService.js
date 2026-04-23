// validationService.js — Mini Project 2
// UNCHANGED from Mini Project 1.
// Client-side validation runs FIRST to prevent unnecessary API calls.
// Email uniqueness is no longer checked here — the API enforces it via 409 Conflict.

const validationService = (() => {

  const validateEmployeeForm = (formData, excludeId = null) => {
    const errors = {};

    if (!formData.firstName || !formData.firstName.trim()) {
      errors.firstName = 'First name is required.';
    }
    if (!formData.lastName || !formData.lastName.trim()) {
      errors.lastName = 'Last name is required.';
    }
    if (!formData.email || !formData.email.trim()) {
      errors.email = 'Email is required.';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email.trim())) {
      errors.email = 'Enter a valid email address.';
    }
    // Note: isEmailTaken() always returns false in MP2 (server enforces it)
    if (!formData.phone || !formData.phone.trim()) {
      errors.phone = 'Phone number is required.';
    } else if (!/^\d{10}$/.test(formData.phone.trim())) {
      errors.phone = 'Phone must be exactly 10 digits.';
    }
    if (!formData.department || formData.department === '') {
      errors.department = 'Department is required.';
    }
    if (!formData.designation || !formData.designation.trim()) {
      errors.designation = 'Designation is required.';
    }
    const salaryRaw = (formData.salary === '' || formData.salary === null || formData.salary === undefined)
      ? '' : String(formData.salary).trim();
    if (salaryRaw === '') {
      errors.salary = 'Salary is required.';
    } else if (isNaN(Number(salaryRaw)) || Number(salaryRaw) <= 0) {
      errors.salary = 'Salary must be a positive number.';
    }
    if (!formData.joinDate || !formData.joinDate.trim()) {
      errors.joinDate = 'Join date is required.';
    }
    if (!formData.status || formData.status === '') {
      errors.status = 'Status is required.';
    }

    return errors;
  };

  const validateAuthForm = (formData, isSignup = false) => {
    const errors = {};

    if (!formData.username || !formData.username.trim()) {
      errors.username = 'Username is required.';
    }
    if (!formData.password || !formData.password.trim()) {
      errors.password = 'Password is required.';
    } else if (isSignup && formData.password.length < 6) {
      errors.password = 'Password must be at least 6 characters.';
    }
    if (isSignup) {
      if (!formData.confirmPassword || !formData.confirmPassword.trim()) {
        errors.confirmPassword = 'Confirm Password is required.';
      } else if (formData.password && formData.password !== formData.confirmPassword) {
        errors.confirmPassword = 'Passwords do not match.';
      }
    }

    return errors;
  };

  return { validateEmployeeForm, validateAuthForm };

})();
