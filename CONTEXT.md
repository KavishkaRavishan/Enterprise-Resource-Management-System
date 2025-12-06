\# Project: Enterprise Resource Management System (ERMS)

\*\*Tech Stack:\*\* .NET 8 Web API, React + Vite, SQL Server, Clean Architecture.



\## Architecture Rules (STRICT)

1\. \*\*Domain Layer\*\*: Pure C# classes. NO external libraries (except basic guards). NO database references.

2\. \*\*Application Layer\*\*: Business logic, Interfaces, DTOs, CQRS (if used). Depends ONLY on Domain.

3\. \*\*Infrastructure Layer\*\*: Database (EF Core), External APIs, Auth implementation. Depends on Application.

4\. \*\*API Layer\*\*: Controllers only. No business logic here. Depends on Application.

5\. \*\*Frontend\*\*: React + Zustand. folder structure: /src/features/{featureName}.



\## Coding Standards

\- \*\*Backend\*\*: Use async/await everywhere. Repository pattern via Dependency Injection.

\- \*\*Frontend\*\*: Functional components. Tailwind for styling. No inline styles.

\- \*\*Error Handling\*\*: Global Exception Handler in .NET.

