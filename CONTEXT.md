\# PROJECT CONTEXT: Enterprise Resource Management System (ERMS)



\## 1. Project Overview

The ERMS is a full-stack web application for managing internal company operations.

\*\*Core Features:\*\*

\- \*\*Auth:\*\* JWT + Refresh Tokens, Role-Based Access (Admin, Manager, Employee).

\- \*\*User Mgmt:\*\* Admin can create/update/delete users and assign roles.

\- \*\*Project Mgmt:\*\* Assign team members, set dates, track progress.

\- \*\*Task Mgmt:\*\* Kanban board (To-Do → In-Progress → Done), comments, real-time status.

\- \*\*Dashboard:\*\* Analytics for users, projects, and tasks.



\## 2. Tech Stack (STRICT)

\- \*\*Frontend:\*\* React + Vite, Tailwind CSS, Zustand (State), React Router, Axios.

\- \*\*Backend:\*\* .NET 8 Web API.

\- \*\*Architecture:\*\* Clean Architecture (Domain, Application, Infrastructure, API).

\- \*\*Database:\*\* SQL Server (EF Core).

\- \*\*Logging:\*\* Serilog.



\## 3. Architecture Rules (DO NOT VIOLATE)

\- \*\*Domain Layer:\*\* Pure C# entities. No dependencies. Private setters for encapsulation.

\- \*\*Application Layer:\*\* Business logic only. Uses DTOs. Interfaces for everything (IRepository).

\- \*\*Infrastructure Layer:\*\* Database implementation (DbContext), Auth logic, External services.

\- \*\*API Layer:\*\* Dumb controllers. They only call the Application layer and return HTTP responses.

\- \*\*Frontend:\*\* Feature-based folder structure (`/src/features/projects`, `/src/features/auth`).



\## 4. Coding Standards

\- \*\*Naming:\*\* PascalCase for C# classes, camelCase for JS/TS variables.

\- \*\*Async:\*\* Use `async/await` for all I/O operations.

\- \*\*State:\*\* Use Zustand for global state (Auth, Theme), local state for forms.

