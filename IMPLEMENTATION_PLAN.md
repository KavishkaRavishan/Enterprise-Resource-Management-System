# ERMS Implementation Plan

## Decisions
- ✅ Seed default admin: `admin@erms.com` / `Admin@123`
- ✅ Support avatar image uploads for user profiles
- ✅ Notifications feature — Option A Polling-based Completed
- ✅ SQL Server: localhost with Windows Auth
- ✅ Tailwind v4 syntax (matches installed package)
- ✅ Polling-free: no real-time features needed

---

## Phase Tracker

| Phase | Description | Status |
|-------|-------------|--------|
| 1 | Domain Layer — Complete Entity Model | ✅ Completed |
| 2 | Application Layer — DTOs, Interfaces, Services | ✅ Completed |
| 3 | Infrastructure Layer — Data Access & Auth | ✅ Completed |
| 4 | API Layer — Controllers, Middleware, Program.cs | ✅ Completed |
| 5 | Database Migration & Seeding | ✅ Completed |
| 6 | Frontend — Core Setup & Auth | ✅ Completed |
| 7 | Frontend — Feature Pages | ✅ Completed |
| 8 | Polish & Final Touches | ✅ Completed |

---

## Phase 1: Domain Layer

### Files:
- [x] Modify `User.cs` — add RefreshToken, IsActive, AvatarPath, nav props
- [x] New `Project.cs`
- [x] New `ProjectMember.cs`
- [x] New `ProjectTask.cs`
- [x] New `TaskComment.cs`
- [x] New `Enums/ProjectStatus.cs`
- [x] New `Enums/TaskItemStatus.cs`
- [x] New `Enums/TaskPriority.cs`

## Phase 2: Application Layer

### Files:
- [x] Delete `Class1.cs`
- [x] DTOs: Auth, Users, Projects, Tasks, Comments, Dashboard
- [x] Interfaces: IAuthService, IUserService, IProjectService, ITaskService, ICommentService, IDashboardService
- [x] Repository Interfaces: IUserRepository, IProjectRepository, ITaskRepository, ICommentRepository
- [x] Services: AuthService, UserService, ProjectService, TaskService, CommentService, DashboardService
- [x] Common: ServiceResult, MappingExtensions

## Phase 3: Infrastructure Layer

### Files:
- [x] Delete `Class1.cs`
- [x] `Data/AppDbContext.cs`
- [x] Entity Configurations (User, Project, ProjectMember, ProjectTask, TaskComment)
- [x] Repositories (User, Project, Task, Comment)
- [x] `Auth/JwtTokenGenerator.cs`, `Auth/PasswordHasher.cs`
- [x] `Seed/DataSeeder.cs`
- [x] `DependencyInjection.cs`

## Phase 4: API Layer

### Files:
- [x] Delete `Class1.cs`
- [x] Change SDK to Web, add Swashbuckle
- [x] `Program.cs`
- [x] `appsettings.json` + `appsettings.Development.json`
- [x] Controllers: Auth, Users, Projects, Tasks, Comments, Dashboard
- [x] `Middleware/ExceptionHandlingMiddleware.cs`

## Phase 5: Database Migration & Seeding
- [x] Run `dotnet ef migrations add InitialCreate`
- [x] Run `dotnet ef database update`

## Phase 6: Frontend — Core Setup & Auth
- [x] Replace stock template
- [x] Axios instance with JWT interceptor
- [x] UI components (Button, Input, Modal, Card, Badge, etc.)
- [x] Layout (Sidebar, Topbar, ProtectedRoute)
- [x] Auth store + Login page
- [x] Routing setup

## Phase 7: Frontend — Feature Pages
- [x] User Management (Admin)
- [x] Project Management
- [x] Task Kanban Board
- [x] Dashboard with analytics
- [x] Avatar upload in profile

## Phase 8: Polish
- [x] Custom dark-friendly/light curated premium color palette
- [x] Responsive layout with collapsible sidebar
- [x] E2E-verified interactive components and loading skeletons
- [x] Clean forms with real-time feedback
- [x] CSS animations and page transition transitions
