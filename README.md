# 🏢 Enterprise Resource Management System (ERMS)

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4.svg?style=for-the-badge&logo=.net&logoColor=white)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18.x-61DAFB.svg?style=for-the-badge&logo=react&logoColor=black)](https://react.dev/)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-3.x-38B2AC.svg?style=for-the-badge&logo=tailwind-css&logoColor=white)](https://tailwindcss.com/)
[![Zustand](https://img.shields.io/badge/Zustand-State-black.svg?style=for-the-badge)](https://github.com/pmndrs/zustand)
[![Entity Framework Core](https://img.shields.io/badge/EF_Core-8.0-512BD4.svg?style=for-the-badge)](https://learn.microsoft.com/en-us/ef/core/)
[![License](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)](LICENSE)

An enterprise-grade, high-performance, full-stack resource and operation management portal designed for internal corporate administration. The system is engineered using **Clean Architecture** and **CQRS (Command Query Responsibility Segregation)** patterns in **.NET 8 Web API**, paired with a modern, modular, and reactive **React + Vite** frontend.

This portal integrates role-based operations, Kanban-style task workflows, dynamic timesheet logging, document attachments, interactive dashboard metrics, automated database audit logging, and automated unit/integration testing.

---

## 📖 Table of Contents

1. [✨ Core Enterprise Modules](#-core-enterprise-modules)
2. [🏛️ Architectural Blueprints](#%EF%B8%8F-architectural-blueprints)
   - [Clean Architecture & Dependency Flow](#clean-architecture--dependency-flow)
   - [CQRS & MediatR Pipeline Flow](#cqrs--mediatr-pipeline-flow)
   - [Authentication & JWT Token Refresh Flow](#authentication--jwt-token-refresh-flow)
   - [Automated Enterprise Change Auditing](#automated-enterprise-change-auditing)
3. [🗄️ Database Relational Schema](#%EF%B8%8F-database-relational-schema)
4. [🔌 REST API Directory](#-rest-api-directory)
5. [📂 Folder Structure & Directory Tree](#-folder-structure--directory-tree)
6. [⚡ Setup & Running Guide](#-setup--running-guide)
   - [Prerequisites](#prerequisites)
   - [Backend Configuration & Execution](#backend-configuration--execution)
   - [Frontend Installation & Running](#frontend-installation--running)
   - [Default Seed Credentials](#default-seed-credentials)
7. [🧪 Testing & Quality Assurance](#-testing--quality-assurance)
8. [📌 Portfolio Highlights](#-portfolio-highlights)
9. [📄 License & Authors](#-license--authors)

---

## ✨ Core Enterprise Modules

The system is split into specialized enterprise modules, ensuring a clean and high-fidelity user experience tailored for corporate management:

*   **🔒 Identity & Role-Based Access Control (RBAC):** Restricts interface access and API execution based on user permissions (`Admin`, `Manager`, `Employee`). Secures routes using high-grade JWT tokens.
*   **📂 Project Management Portal:** Allows Managers and Admins to define scopes, establish deadlines, allocate operational budgets, and map cross-departmental user assignments.
*   **📋 Interactive Kanban Board:** Facilitates drag-and-drop task tracking, showing task progress categorized by status (`To Do`, `In Progress`, `Done`), assignment cards, and priority badges (`Low`, `Medium`, `High`).
*   **💬 Collaborative Task Threads:** Implements lightweight task-level forums, permitting project members to add and remove comments directly under active workspace task cards.
*   **📎 Document & File Repository:** Supports direct local uploads for files associated with tasks, including automated physical disk cleanup and validation of MIME content types.
*   **⏱️ Dynamic Timesheets:** Enables employees to log granular hours worked against assigned tasks, outputting interactive telemetry of weekly/monthly utilization.
*   **🔔 Real-Time Action Notifications:** Employs persistent in-app notifications to immediately alert employees when they are assigned tasks or projects.
*   **📈 Dashboard & Business Intelligence (BI):** Incorporates clean data visualizations powered by **Recharts**, illustrating task status distribution and project resource workload analytics.
*   **🛡️ Compliance & Audit Trail:** Intercepts database writes to record a read-only historical timeline of changes (displaying old values vs. new values in JSON) for full administrative auditing.

---

## 🏛️ Architectural Blueprints

### Clean Architecture & Dependency Flow

The system is separated into four layers following Clean Architecture guidelines. This guarantees that the core Domain models and business policies remain highly isolated and independent of databases, user interfaces, or third-party libraries.

```
                  ┌─────────────────────────────────────────┐
                  │               ERMS.API                  │
                  │ (Controllers, Middlewares, Program.cs)   │
                  └────────────┬───────────────┬────────────┘
                                │               │
                                ▼               ▼
         ┌───────────────────────────┐     ┌───────────────────────────┐
         │     ERMS.Infrastructure   │     │      ERMS.Application     │
         │(EF Core DbContext, Auth,  │────>│(CQRS Handlers, Interfaces,│
         │ Repositories, FileStorage)│     │     DTOs, Validations)    │
         └───────────────────────────┘     └────────────┬──────────────┘
                                                        │
                                                        ▼
                                           ┌───────────────────────────┐
                                           │        ERMS.Domain        │
                                           │  (Entities, Enums, Rules) │
                                           └───────────────────────────┘
```

*   **Domain Layer:** Fully self-contained and pure. It defines all system models (e.g. `User`, `Project`, `ProjectTask`), constraints, and custom enumerations. No external reference to databases or NuGet packages exists here.
*   **Application Layer:** Contains the application-specific business logic. It establishes interfaces for repositories/services, houses all DTOs (Data Transfer Objects), and defines the CQRS commands, queries, handlers, and validation rules.
*   **Infrastructure Layer:** Implements the interfaces defined in the Application layer. It handles EF Core, SQL Server database persistence, JWT cryptography, local disk attachment storage, and audit logs.
*   **API Layer:** The entry point. Handles HTTP routing, JWT middleware validation, cross-origin resource sharing (CORS), Swagger configuration, and maps incoming requests directly to MediatR pipeline queries/commands.

---

### CQRS & MediatR Pipeline Flow

Rather than using classic bloated service layers, the project relies on **CQRS (Command Query Responsibility Segregation)** via **MediatR** for modifying and reading database records. This separates operations into **Commands** (Write/Modify) and **Queries** (Read).

In addition, an **Automated FluentValidation Pipeline Behavior** intercepts every command/query before reaching its handler, throwing validation errors immediately if request rules are violated.

```
Incoming Request
      │
      ▼
 ┌─────────┐
 │ API Ctr │
 └────┬────┘
      │
      ▼ (Send Command/Query)
 ┌─────────┐
 │ MediatR │
 └────┬────┘
      │
      ▼ (Pipeline Interception)
 ┌────────────────────────────────────────────────────────┐
 │ ValidationBehavior: Runs all Registered Validators     │
 └──────────────────────────┬─────────────────────────────┘
                            ├─────────────────────────┐
                            ▼ (Failures Exist)        ▼ (All Valid)
                     ┌───────────────┐         ┌─────────────┐
                     │ Throw Except  │         │ Run Handler │
                     └───────────────┘         └──────┬──────┘
                                                      │
                                                      ▼ (DbContext SaveChanges)
                                               ┌─────────────┐
                                               │ EF Context  │
                                               └──────┬──────┘
                                                      │
                                                      ▼ (Intercept)
                                               ┌─────────────┐
                                               │ Audit Logs  │
                                               └──────┬──────┘
                                                      ▼
                                                 Database Save
```

---

### Authentication & JWT Token Refresh Flow

The authentication system employs highly secure JWT (JSON Web Tokens) supplemented by a Refresh Token model. The frontend receives an `accessToken` (short-lived, 15 minutes) and a `refreshToken` (long-lived, 7 days). 

Using a custom **Axios Interceptor** pool on the client, when a token expires and returns a `401 Unauthorized` response, the system dynamically pauses the request queue, requests new tokens, and transparently retries the initial user request without interrupting the user experience.

```
   Client (React Store)                     API Server (Backend)
          │                                           │
          ├────────── Send Access Token ─────────────>┤
          │                                           │ (Expired!)
          │<────────── Return 401 Unauthorized ───────┤
          │                                           │
          │         [Axios Interceptor Catches 401]   │
          ├────────── Post Refresh Token ────────────>┤
          │                                           │ (Validates DB Entry)
          │<────────── Return New Tokens ─────────────┤
          │                                           │
          │         [Retry Intercepted HTTP Request]  │
          ├────────── Re-Send Original Request ──────>┤
          │                                           │ (Success!)
          │<────────── Return HTTP Data ──────────────┤
```

---

### Automated Enterprise Change Auditing

To maintain dynamic corporate compliance and operational transparency, the database context uses a custom Entity Framework Core `SaveChangesInterceptor`. 

Every time a write, update, or delete transaction takes place, the system intercepts the database write, analyzes the EF Change Tracker, automatically serializes the changed properties into JSON blocks (`OldValues` vs `NewValues`), maps the active operating User ID from the session context, and persists the entry directly to the SQL `AuditLogs` table.

> [!NOTE]
> This happens transparently and globally. Developers do not need to manually write audit tracking code inside their business logic or controller handlers.

---

## 🗄️ Database Relational Schema

The database relational schema is configured using Entity Framework Core's Fluent API configurations inside the infrastructure layer. Below is the relational structure:

```mermaid
erDiagram
    User ||--o{ ProjectMember : belongs
    User ||--o{ TimeLog : logs
    User ||--o{ Notification : receives
    User ||--o{ AuditLog : triggers
    User ||--o{ TaskComment : writes
    
    Project ||--o{ ProjectMember : has
    Project ||--o{ ProjectTask : contains
    
    ProjectTask ||--o{ TaskComment : has
    ProjectTask ||--o{ Attachment : holds
    ProjectTask ||--o{ TimeLog : has
    
    User {
        Guid Id PK
        string Email UK
        string PasswordHash
        string FirstName
        string LastName
        string Role
        bool IsActive
    }
    
    Project {
        Guid Id PK
        string Name
        string Description
        DateTime StartDate
        DateTime EndDate
        string Status
    }
    
    ProjectMember {
        Guid Id PK
        Guid ProjectId FK
        Guid UserId FK
    }
    
    ProjectTask {
        Guid Id PK
        Guid ProjectId FK
        Guid AssignedUserId FK
        string Title
        string Description
        string Status
        string Priority
        DateTime? DueDate
    }
    
    TaskComment {
        Guid Id PK
        Guid TaskId FK
        Guid UserId FK
        string Content
        DateTime CreatedAt
    }
    
    Attachment {
        Guid Id PK
        Guid TaskId FK
        string FileName
        string FilePath
        long SizeBytes
        string ContentType
        DateTime UploadedAt
    }
    
    TimeLog {
        Guid Id PK
        Guid TaskId FK
        Guid UserId FK
        double HoursSpent
        string Description
        DateTime DateLogged
    }
    
    AuditLog {
        Guid Id PK
        string EntityName
        string EntityId
        string Action
        string OldValues
        string NewValues
        Guid UserId FK
        DateTime Timestamp
    }
    
    Notification {
        Guid Id PK
        Guid UserId FK
        string Title
        string Message
        string Type
        bool IsRead
        DateTime CreatedAt
    }
```

### Relational Configuration & Integrity:
1.  **Users & Projects:** Connected through `ProjectMembers` enabling many-to-many relationship tracking.
2.  **Tasks & Cascades:** `ProjectTasks` are linked to `Projects` with a foreign key and cascade-delete options. A Task can be assigned to exactly one `User`.
3.  **File Attachments:** Linked directly to `ProjectTasks`. Upon task removal, EF Core cascades the deletion to clean up attachment references.
4.  **Audit Trail logs:** Tracks structural mutations with serialized JSON snapshots of database state differences.

---

## 🔌 REST API Directory

The backend exposes a highly structured, secure, and resource-oriented REST API. All endpoints (except where marked otherwise) require a valid JWT Access Token passed in the `Authorization: Bearer <token>` header.

| Module | Endpoint | HTTP Method | Auth Level | Description |
| :--- | :--- | :---: | :--- | :--- |
| **Authentication** | `/api/auth/login` | `POST` | Public | Authenticates credentials and returns a JWT access token and refresh token. |
| | `/api/auth/register` | `POST` | Public | Registers a new employee profile in the system. |
| | `/api/auth/refresh` | `POST` | Public | Rotates the expired JWT token using a valid refresh token. |
| **Users** | `/api/users` | `GET` | Admin, Manager | Lists all registered users and employee profiles. |
| | `/api/users/profile` | `GET` | All Roles | Retrieves the currently authenticated user's profile detail. |
| | `/api/users/{id}/status` | `PUT` | Admin | Enables/disables an employee account to revoke system access. |
| **Projects** | `/api/projects` | `GET` | All Roles | Lists projects the authenticated user is assigned to. |
| | `/api/projects` | `POST` | Admin, Manager | Creates a new corporate project. |
| | `/api/projects/{id}` | `GET` | All Roles | Retrieves full details of a specific project. |
| | `/api/projects/{id}` | `PUT` | Admin, Manager | Updates project description, status, or date timelines. |
| | `/api/projects/{id}` | `DELETE` | Admin | Permanently deletes a project and cascades deletions. |
| | `/api/projects/{id}/members` | `POST` | Admin, Manager | Assigns project members to a project. |
| **Tasks** | `/api/tasks/project/{projectId}` | `GET` | All Roles | Fetches all tasks associated with a project. |
| | `/api/tasks` | `POST` | Admin, Manager | Creates and assigns a new task within a project. |
| | `/api/tasks/{id}` | `GET` | All Roles | Retrieves granular details of a specific task. |
| | `/api/tasks/{id}` | `PUT` | All Roles | Updates a task status (Kanban move), priority, or title. |
| | `/api/tasks/{id}` | `DELETE` | Admin, Manager | Deletes a task from the board. |
| **Comments** | `/api/comments/task/{taskId}` | `GET` | All Roles | Retrieves task discussion history. |
| | `/api/comments` | `POST` | All Roles | Post a new comment under a specific task. |
| | `/api/comments/{id}` | `DELETE` | Author, Admin | Deletes a previously written comment. |
| **Attachments** | `/api/attachments/task/{taskId}` | `GET` | All Roles | Lists all uploaded file metadata for a specific task. |
| | `/api/attachments` | `POST` | All Roles | Uploads a local file attachment associated with a task. |
| | `/api/attachments/{id}/download` | `GET` | All Roles | Downloads the physical attachment file from the disk store. |
| | `/api/attachments/{id}` | `DELETE` | All Roles | Removes file attachment record and physical file. |
| **Time Logs** | `/api/timelogs/task/{taskId}` | `GET` | All Roles | Retrieves time allocation log for a specific task. |
| | `/api/timelogs` | `POST` | All Roles | Logs hours worked and tasks completed into timesheets. |
| | `/api/timelogs/user/{userId}` | `GET` | All Roles | Retrieves time logging history for a specific employee. |
| **Notifications** | `/api/notifications` | `GET` | All Roles | Fetches user notifications (e.g. assignment alerts). |
| | `/api/notifications/{id}/read` | `PUT` | All Roles | Marks a specific notification as read. |
| **Dashboard** | `/api/dashboard/stats` | `GET` | Admin, Manager | Retrieves system-wide aggregates and analytics widgets. |
| **Audit Logs** | `/api/auditlogs` | `GET` | Admin | Retrieves system compliance logs (Old vs New values). |

---

## 📂 Folder Structure & Directory Tree

The solution workspace is highly organized, using distinct directories for .NET clean architecture and the modular frontend client:

```
Enterprise-Resource-Management-System/
│
├── ERMS.sln                          # Master Visual Studio Solution File
│
├── ERMS.Domain/                      # Core Business Layer (Entities & Rules)
│   ├── Entities/                     # User, Project, ProjectTask, Attachment, TimeLog, AuditLog, etc.
│   └── Enums/                        # NotificationType, ProjectStatus, TaskPriority, etc.
│
├── ERMS.Application/                 # Service Interfaces & Core CQRS Logic
│   ├── Common/                       # ValidationBehavior.cs, ServiceResult.cs
│   ├── DTOs/                         # AuthDto, ProjectDto, TaskDto, TimeLogDto, AttachmentDto, etc.
│   ├── Interfaces/                   # Repository & Service interface declarations
│   ├── Services/                     # Base service implementations
│   └── Tasks/                        # Task CQRS Segment
│       ├── Commands/                 # CreateTaskCommand, UpdateTaskCommand, DeleteTaskCommand, etc.
│       └── Queries/                  # GetTasksByProjectQuery, GetTaskByIdQuery, etc.
│
├── ERMS.Infrastructure/              # DB Context, Repositories, Auth & Outer Services
│   ├── Auth/                         # Token Generator, Password Hashing
│   ├── Data/                         # EF Context & Fluent API Configurations
│   │   ├── Configurations/           # Fluent Mappings for User, Project, Task, TimeLog, etc.
│   │   └── Interceptors/             # AuditSaveChangesInterceptor.cs
│   ├── Migrations/                   # EF Core SQL Database Migrations
│   ├── Repositories/                 # Concrete SQL repositories (UserRepository, ProjectRepository, etc.)
│   └── Services/                     # Storage service (Disk saves), Http Session context
│
├── ERMS.API/                         # API Hosting Layer (Controllers & Config)
│   ├── Controllers/                  # Auth, Projects, Tasks, TimeLogs, Attachments, AuditLogs, etc.
│   ├── Middleware/                   # Custom Exception & Request handling
│   ├── wwwroot/                      # Static hosting directory for uploads & files
│   └── Program.cs                    # Server startup configuration
│
├── ERMS.Tests/                       # Test Suite (xUnit, Moq, FluentValidation)
│   ├── Domain/                       # Domain entity validation unit tests
│   ├── Queries/                      # CQRS Queries handler mock tests
│   └── Validation/                   # Command Validator rule assertion tests
│
└── ERMS.Client/                      # Frontend Client Application (React + Vite)
    ├── package.json                  # Frontend scripts and dependency versions
    ├── vite.config.js                # Vite build parameters
    └── src/                          # Frontend source
        ├── api/                      # Axios HTTP client instantiation & interceptors
        ├── features/                 # Feature-based modular slices (auth, dashboard, tasks, timelogs, etc.)
        ├── components/               # Global layout components (NavBar, Sidebar, Skeletons)
        └── index.css                 # Master CSS styles and custom UI styling
```

---

## ⚡ Setup & Running Guide

Follow these steps to run the complete environment locally.

### Prerequisites:
*   **SDK:** [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
*   **Runtime Environment:** [Node.js (LTS v18 or later)](https://nodejs.org/)
*   **Database Server:** Local SQL Server Express or SQL Server LocalDB.

---

### Backend Configuration & Execution:

1.  **Configure Connection Strings:**
    Open `ERMS.API/appsettings.json` and set your local SQL Server address:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ErmsDatabase;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```
2.  **Execute Database Migrations:**
    Install `dotnet-ef` global tool if you haven't already:
    ```powershell
    dotnet tool install --global dotnet-ef
    ```
    Apply migrations to build the SQL tables:
    ```powershell
    cd ERMS.API
    dotnet ef database update
    ```
3.  **Start the API Server:**
    ```powershell
    dotnet run
    ```
    Once started, the backend automatically seeds default system accounts, and Swagger is accessible at `http://localhost:5000/swagger` (or defined HTTPS port).

---

### Frontend Installation & Running:

1.  **Navigate to the Client Directory:**
    ```powershell
    cd ERMS.Client
    ```
2.  **Install Frontend Dependencies:**
    ```powershell
    npm install
    ```
3.  **Launch the Client Dev Server:**
    ```powershell
    npm run dev
    ```
    Open your browser to `http://localhost:5173`.

---

### Default Seed Credentials:
Upon initial database configuration, the system seeds three default operational accounts:
*   **Administrator:** `admin@erms.com` / `Admin@123`
*   **Manager:** `manager@erms.com` / `Manager@123`
*   **Employee:** `employee@erms.com` / `Employee@123`

---

## 🧪 Testing & Quality Assurance

The project includes an **xUnit** test project containing unit, database mock, and validation pipeline tests.

*   **Mock Repository Assertions:** Uses `Moq` to verify service layer outputs without interacting with physical database tables.
*   **FluentValidation Test Suite:** Validates business rule exceptions for CQRS requests.
*   **Domain Business Rules:** Tests Domain entity creation logic (such as enforcing positive logs on Timesheets).

Run tests from the solution's root directory:
```powershell
dotnet test
```

---

## 📌 Portfolio Highlights

This project demonstrates production-ready enterprise engineering skills, showcasing:
*   **Domain-Driven Integrity:** Strict domain encapsulation with internal setters, ensuring state integrity through pure domain entities.
*   **MediatR Vertical Segregation:** High cohesion and separation of concerns via the CQRS pattern, keeping queries and commands logically decoupled.
*   **Pipelined Validation Middleware:** Prevents dirty data from hitting database channels by running commands through automated validation filters before processing.
*   **Dynamic Data Telemetry:** A visual management console incorporating interactive dashboards (`Recharts`) and fluid board management (`Vite` + `Zustand`).
*   **Automatic SaveChanges Auditing:** Implementation of custom `SaveChangesInterceptor` to log full object mutations for regulatory compliance.

---

## 📄 Author

**Kavishka Ravishan**  
Computer Engineering Undergraduate

- GitHub: [@KavishkaRavishan](https://github.com/KavishkaRavishan)
- LinkedIn: [Kavishka Ravishan](https://www.linkedin.com/in/kavishka-ravishan/)
