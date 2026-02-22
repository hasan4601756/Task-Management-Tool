# Task Management Tool

## Project Description
**Task Management Tool** is a full-stack web application built with ASP.NET Core and React that enables secure task creation, assignment, and tracking with role-based access control, logging via Serilog, SQL database integration, unit testing, and code quality analysis using SonarQube.

---

## Technology Stack

### Backend
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Serilog
- xUnit

### Frontend
- React.js
- JavaScript
- CSS

### Tools & DevOps
- Git
- SonarQube

---

## Key Features

### User Authentication & Authorization
- User registration and login
- JWT-based authentication
- Role-based access control (Admin, User)

### Task Management
- Create, read, update, and delete tasks
- Assign tasks to users
- Set task priority, category, and due dates
- Track task status (Pending, In Progress, Completed)

### Dashboard
- Displays task counts by status
- Admin users can view all tasks
- Regular users see only assigned tasks

### User Profile
- View user details
- Secure logout functionality

### Logging & Exception Handling
- Centralized logging using Serilog
- Global exception handling
- Error tracking and diagnostics

### Testing & Code Quality
- Unit testing using xUnit
- Static code analysis with SonarQube (SonarCloud)

---

## Application Screens

| Screen        | Description |
|--------------|-------------|
| Sign Up / Login | User authentication |
| Dashboard | Task statistics overview |
| Task List | View and filter tasks |
| Task Detail | Detailed task information |
| New Task | Create or update tasks |
| User Profile | User information and logout |

---

## How to Run the Project

### Backend
```bash
cd backend
dotnet restore
dotnet ef database update --project TaskManagementTool.Infrastructure --startup-project TaskManagementTool.API
dotnet run --project TaskManagementTool.API
```

### Frontend
```bash
cd frontend
npm install
npm run dev
```

## Future Enhancements

#### Real-time task updates using SignalR

#### Advanced search and filtering

#### Task import/export functionality

## Author
### Muhammad Ahmed Hasan
