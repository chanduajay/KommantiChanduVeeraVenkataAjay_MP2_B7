# Employee Management System — Mini Project 2

**Name:** Kommanti Chandu Veera Venkata Ajay

**Batch:** 7  .Net With Python

**Submission:** Batch7_.Net-With-Python_Kommanti-Chandu-Veera-Venkata-Ajay_EmployeeManagementSystem_MiniProject_2

---

## Tech Stack

- **Backend:** .NET 8 Web API, Entity Framework Core 8 (Code First), SQL Server 2022
- **Authentication:** BCrypt.Net-Next (12 rounds), JWT Bearer (8-hour tokens)
- **Frontend:** HTML5, CSS3, Bootstrap 5, JavaScript ES6+, jQuery 3.x
- **Tests:** NUnit 3, Moq 4 (no real database — InMemory for AuthService, Mock for EmployeeService)

---

## Project Structure

```
EMS-MiniProject2/
├── EMS.sln
├── README.md
├── EMS.API/                          ← .NET 8 Web API
│   ├── Controllers/
│   │   ├── AuthController.cs         ← POST /api/auth/register + /api/auth/login
│   │   └── EmployeesController.cs    ← CRUD + /dashboard + pagination
│   ├── Data/
│   │   └── AppDbContext.cs           ← EF Core DbContext + seed data
│   ├── DTOs/
│   │   └── EmployeeDtos.cs           ← request/response/paged DTOs
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IEmployeeRepository.cs    ← contract Moq mocks in tests
│   │   └── IEmployeeService.cs
│   ├── Models/
│   │   ├── AppUser.cs
│   │   └── Employee.cs
│   ├── Services/
│   │   ├── AuthService.cs            ← BCrypt hashing + JWT generation
│   │   ├── EmployeeRepository.cs     ← server-side filter/sort/pagination
│   │   └── EmployeeService.cs
│   ├── Properties/
│   │   └── launchSettings.json       ← port 5000
│   ├── appsettings.json              ← connection string + JWT config
│   └── Program.cs                    ← DI, CORS, Swagger, JWT middleware
├── EMS.Tests/                        ← NUnit test project
│   ├── EmployeeServiceTests.cs       ← 8 tests, pure Moq
│   └── AuthServiceTests.cs           ← 5 tests, InMemory DB
└── frontend/                         ← Updated from Mini Project 1
    ├── index.html
    ├── css/
    │   └── styles.css
    └── js/
        ├── config.js                 ← NEW: API_BASE_URL constant
        ├── storageService.js         ← REPLACED: real fetch() API calls
        ├── authService.js            ← UPDATED: calls /api/auth/*
        ├── employeeService.js        ← UPDATED: async delegates
        ├── validationService.js      ← UNCHANGED from Mini Project 1
        ├── dashboardService.js       ← UPDATED: single API call
        ├── uiService.js              ← UPDATED: pagination + role UI
        └── app.js                    ← UPDATED: async/await + pagination state
```

---

## How to Run

### Step 1 — Prerequisites
- Visual Studio 2022 (v17+)
- .NET 8 SDK
- SQL Server 2022
- VS Code with Live Server extension (for frontend)

### Step 2 — Open Solution
Open `EMS.sln` in Visual Studio 2022.

### Step 3 — Run EF Migrations (REQUIRED before first run)

In **Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console),
make sure **Default Project** is set to `EMS.API`:

(instance: `LAPTOP-MLTJVSB2\SQLSERVER`) - Default instance used in this project: `LAPTOP-MLTJVSB2\SQLSERVER`
- ⚠️ You must update this to your own SQL Server instance
- ### ⚙️ Configure Connection String

Open `EMS.API/appsettings.json` and verify the connection string: `"DefaultConnection": "Server=LAPTOP-MLTJVSB2\\SQLSERVER;Database=EMS;Trusted_Connection=True;TrustServerCertificate=True;"`. 
⚠️ Make sure to replace `LAPTOP-MLTJVSB2\\SQLSERVER` with your own SQL Server instance name. If you are using a default instance, set it as `Server=YourMachineName`, and if you are using a named instance like SQLEXPRESS, set it as `Server=YourMachineName\\SQLEXPRESS`. Example: `"DefaultConnection": "Server=DESKTOP-12345\\SQLEXPRESS;Database=EMS;Trusted_Connection=True;TrustServerCertificate=True;"`.

```
Add-Migration InitialCreate
Update-Database
```

This creates the `EMSDashboard` database with:
- `Employees` table — 15 seeded records (identical to Mini Project 1 data.js)
- `Users` table — 2 seeded accounts:
  - `admin / admin123` → Role: Admin (full CRUD)
  - `viewer / viewer123` → Role: Viewer (read-only)

### Step 4 — Run the API

Press **F5** in Visual Studio (select `http` profile).
API starts at: `http://localhost:5000`
Swagger UI: `http://localhost:5000/swagger`

### Step 5 — Open the Frontend

In VS Code, right-click `frontend/index.html` → **Open with Live Server** (port 5500).
Or open `frontend/index.html` directly in Chrome — both origins are whitelisted in CORS.


## Commands to Run

### Backend (from EMS.API directory):
dotnet ef database update
dotnet run

### Or from published folder:
dotnet EMS.API.dll

### Frontend:
Open frontend/index.html with VS Code Live Server (port 5500)
Or open frontend/index.html directly in Chrome

---

## Default Credentials

| Username | Password  | Role   | Access              |
|----------|-----------|--------|---------------------|
| admin    | admin123  | Admin  | Full CRUD           |
| viewer   | viewer123 | Viewer | Read-only           |

---

## Running Tests

```
# Visual Studio: Test Explorer → Run All
# CLI:
dotnet test EMS.Tests
```

---

## API Endpoints

| Method | Endpoint                       | Auth    | Role   | Description                    |
|--------|-------------------------------|---------|--------|--------------------------------|
| POST   | /api/auth/register            | No      | —      | Register new user              |
| POST   | /api/auth/login               | No      | —      | Login, returns JWT             |
| GET    | /api/employees/dashboard      | Bearer  | Any    | All KPIs in one call           |
| GET    | /api/employees                | Bearer  | Any    | Paged list with search/filter  |
| GET    | /api/employees/{id}           | Bearer  | Any    | Get single employee            |
| POST   | /api/employees                | Bearer  | Admin  | Create employee                |
| PUT    | /api/employees/{id}           | Bearer  | Admin  | Update employee                |
| DELETE | /api/employees/{id}           | Bearer  | Admin  | Delete employee                |
