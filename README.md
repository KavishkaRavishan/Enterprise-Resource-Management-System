# Enterprise Resource Management System (ERMS)

**Enterprise Resource Management System (ERMS)** is a full-stack web application designed to manage internal company operations, including users, projects, tasks, and notifications. It features role-based access control, a Kanban task board, and an admin dashboard with analytics.

---

## 🚀 Key Features

### Authentication & Authorization
- JWT + Refresh tokens
- Role-Based Access Control: Admin, Manager, Employee
- Protected routes on frontend

### User Management (Admin)
- Create, update, delete users
- Assign roles
- User profile management

### Project Management (Admin & Manager)
- Create and manage projects
- Assign team members
- Set start/end dates
- Track project progress

### Task Management (Manager & Employees)
- Kanban board (To-Do → In-Progress → Done)
- Assign tasks to users
- Set priority & deadlines
- Commenting system
- Real-time status updates (in-app notifications)

### Admin Dashboard
- Total users, active projects, pending tasks
- Recent activity
- Simple charts and tables

### Notifications
- In-app alerts for task assignments and status changes
- Mark-as-read support

---

## 🛠️ Tech Stack

### Frontend
- React + Vite
- Tailwind CSS
- React Router
- Axios for API communication

### Backend
- .NET 8 Web API
- Clean Architecture (Domain, Application, Infrastructure layers)
- Entity Framework Core
- JWT Authentication
- Serilog for logging

### Database
- SQL Server
- Well-structured relational schema with foreign key relationships

---

## 📂 Folder Structure (Overview)

### Frontend
src/
api/
components/
hooks/
pages/
stores/
layouts/
utils/

### Backend (Clean Architecture)
Erms.Api/
Erms.Application/
Erms.Domain/
Erms.Infrastructure/

---

## ⚡ How to Run

### Backend
1. Navigate to `Erms.Api`
2. Update `appsettings.json` with your SQL Server connection
3. Run migrations:
dotnet ef database update

4. Start API:
dotnet run


### Frontend
1. Navigate to `erms-frontend`
2. Install dependencies:
npm install

3. Start development server:
npm run dev


---

## 🌟 Portfolio Highlights
- Professional, real-world application
- Full-stack implementation (React + .NET 8 + SQL Server)
- Clean Architecture
- Role-based access and dashboards
- Kanban board for tasks

---

## 📌 Author
**Kavishka Ravishan**  
- GitHub: [https://github.com/KavishkaRavishan](https://github.com/KavishkaRavishan)  
- LinkedIn: [https://www.linkedin.com/in/kavishka-ravishan/](https://www.linkedin.com/in/kavishka-ravishan/)  

---

## 📄 License
This project is **open-source** and free to use for learning purposes.
