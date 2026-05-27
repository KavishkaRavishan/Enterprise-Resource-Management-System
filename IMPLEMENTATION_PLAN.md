# ERMS Enterprise Upgrade Plan

This implementation plan outlines the steps to upgrade the Enterprise Resource Management System (ERMS) to an industry-grade platform, featuring Timesheet Time Logging, Document Attachments, advanced Recharts metrics, CQRS MediatR architecture, and full system testing.

---

## Decisions
- ✅ Database Strategy: SQL Server Localhost
- ✅ File Storage Strategy: Local filesystem storage under `ERMS.API/wwwroot/uploads/attachments`
- ✅ Dynamic Audit Trails: Automated EF Core Auditing Interceptor
- ✅ Architectural Refactoring: Step-by-step CQRS integration

---

## Phase Tracker

| Phase | Description | Status |
|-------|-------------|--------|
| 1 | Timesheets & Time Logging (Database, API, Frontend) | ✅ Completed |
| 2 | Project & Task Document Management (File Uploads) | ✅ Completed |
| 3 | Premium Dashboard Metrics & Interactive Charts (Recharts) | ✅ Completed |
| 4 | Clean CQRS Refactor (MediatR & Pipeline Validation) | ✅ Completed |
| 5 | Global System Audit Logging (EF Core Interceptor) | ✅ Completed |
| 6 | System Testing (xUnit Test Suite) | ✅ Completed |

---

## Phase 1: Timesheets & Time Logging

### Backend
- [x] Create `TimeLog.cs` entity in Domain layer (TaskId, UserId, HoursSpent, Description, DateLogged)
- [x] Create `TimeLogConfiguration.cs` fluent mapping configuration in Infrastructure layer
- [x] Create `ITimeLogRepository.cs` and implement `TimeLogRepository.cs`
- [x] Create DTOs: `TimeLogDto.cs`, `LogTimeDto.cs`
- [x] Create `ITimeLogService.cs` and implement `TimeLogService.cs`
- [x] Add `TimeLogsController.cs` in API project
- [x] Apply EF Core migration `AddTimesheets` and update database

### Frontend
- [x] Create `useTimeLogStore.js` for timesheet state management
- [x] Add "Log Time" section/modal in Kanban task detail view
- [x] Add a timesheet history listing inside the task detail pane
- [x] Display aggregated total logged hours on Project Details dashboard

---

## Phase 2: Document Management & File Attachments

### Backend
- [x] Create `Attachment.cs` entity in Domain layer (FileName, Size, FilePath, ContentType, TaskId/ProjectId)
- [x] Configure EF mappings for Attachments (cascade delete on task removal)
- [x] Implement `IAttachmentRepository.cs` and `AttachmentRepository.cs`
- [x] Implement `IFileStorageService.cs` saving attachments safely to `ERMS.API/wwwroot/uploads/attachments/`
- [x] Add `AttachmentsController.cs` supporting `POST upload` and `GET download` operations
- [x] Apply EF Core migration `AddAttachments` and update database

### Frontend
- [x] Create `useAttachmentStore.js` for managing file uploads
- [x] Add drag-and-drop file attachment box inside Task Details modal
- [x] List uploaded attachments inside task panels with preview/download links

---

## Phase 3: Premium Dashboard Metrics & Interactive Charts

### Frontend
- [x] Install `recharts` package inside `ERMS.Client`
- [x] Re-engineer Dashboard widgets to include interactive graphs:
  - [x] Pie/Donut Chart showing Task distribution by state (`To Do`, `In Progress`, `Done`)
  - [x] Bar Chart illustrating logged employee timesheet hours per project
- [x] Add loading skeletons and entrance animations

---

## Phase 4: Clean CQRS Architecture Refactor

### Backend
- [x] Add NuGet packages: `MediatR`, `FluentValidation.DependencyInjectionExtensions`
- [x] Create Pipeline validation behavior: `ValidationBehavior.cs`
- [x] Refactor one core domain segment (e.g. `Tasks`) to use MediatR:
  - [x] Create commands: `CreateTaskCommand`, `UpdateTaskCommand`, `DeleteTaskCommand`
  - [x] Create queries: `GetTasksByProjectQuery`, `GetTaskByIdQuery`
  - [x] Write request validators using FluentValidation

---

## Phase 5: Global System Audit Logging

### Backend
- [x] Create `AuditLog.cs` entity in Domain layer (EntityName, EntityId, Action, Timestamp, OldValues, NewValues, UserId)
- [x] Implement EF Core `SaveChangesInterceptor` to capture dirty changes automatically during entity saves
- [x] Configure Context to auto-persist audit records upon save operations
- [x] Expose an Admin-only audit trail dashboard in the frontend

---

## Phase 6: System Testing Suite

### Backend Tests
- [x] Create `ERMS.Tests` xUnit test project
- [x] Write repository mock tests using `Moq`
- [x] Write domain model unit tests validating business rules
- [x] Verify validation pipelines using unit test cases
