// authService.js — Mini Project 2
// UPDATED: calls real API endpoints via storageService.
// JWT stored in a JavaScript variable (in-memory) ONLY.
// NEVER written to localStorage or sessionStorage (XSS protection).
// Token is lost on page refresh — requires re-login (correct secure behaviour).

const authService = (() => {

  // In-memory session — { username, role, token }
  let _session = null;

  // ── Register ──────────────────────────────────────────────────────────────
  const signup = async (username, password, role = 'Admin') => {
    try {
      const result = await storageService.register({ username, password, role });
      return { success: true, data: result };
    } catch (err) {
      if (err.status === 409) {
        return { success: false, error: 'username_taken', message: err.message };
      }
      return { success: false, error: 'server_error', message: err.message };
    }
  };

  // ── Login ─────────────────────────────────────────────────────────────────
  const login = async (username, password) => {
    try {
      const result = await storageService.login({ username, password });
      if (result && result.success) {
        // Store token + role in-memory only
        _session = {
          username: result.username,
          role:     result.role,
          token:    result.token
        };
        return { success: true };
      }
      return { success: false };
    } catch (err) {
      return { success: false, message: err.message };
    }
  };

  // ── Logout ────────────────────────────────────────────────────────────────
  const logout = () => {
    _session = null;
  };

  // ── Session accessors ─────────────────────────────────────────────────────
  const isLoggedIn    = () => _session !== null;
  const getCurrentUser = () => _session?.username || null;
  const getToken       = () => _session?.token     || null;
  const getRole        = () => _session?.role      || null;
  const isAdmin        = () => _session?.role === 'Admin';

  return { signup, login, logout, isLoggedIn, getCurrentUser, getToken, getRole, isAdmin };

})();
