// employeeService.js — Mini Project 2
// Architecture UNCHANGED from Mini Project 1.
// Same method names, same signatures.
// The only difference: storageService methods now return Promises (HTTP calls),
// so callers in app.js use await — but this file itself is unchanged.

const employeeService = (() => {

  // In MP2 getAll() accepts queryParams for server-side search/filter/sort/page
  const getAll = (queryParams = {}) => storageService.getAll(queryParams);

  const getById = (id) => storageService.getById(id);

  const add = (data) => storageService.add(data);

  const update = (id, data) => storageService.update(id, data);

  const remove = (id) => storageService.remove(id);

  // Email uniqueness is now enforced server-side (409 Conflict).
  // This stub keeps validationService.js working without changes.
  const isEmailTaken = () => false;

  // Static department list — used only to populate the filter dropdown.
  // In MP2 the server is the authority; this matches the seeded departments.
  const getDepartments = () =>
    ['Engineering', 'Finance', 'HR', 'Marketing', 'Operations'];

  return { getAll, getById, add, update, remove, isEmailTaken, getDepartments };

})();
