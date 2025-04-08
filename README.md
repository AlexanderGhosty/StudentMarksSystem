
# Grade Management System

A complete solution for managing students, subjects, and grades in an educational organization. The system consists of:
- A **PostgreSQL** database to store data about users (teachers, students, admins), subjects, and grades.
- A **GoServer** application (written in Go + Gin) that provides RESTful API endpoints and handles authentication, role-based authorization, and database access.
- A **WPF Client** (C#/.NET) that offers a user-friendly desktop interface for administrators, teachers, and students.

This documentation provides instructions for **setting up** and **running** the system, as well as an **overview** of its architecture and main features.

---

## Table of Contents

1. [Features](#features)
2. [System Architecture](#system-architecture)
3. [Technology Stack](#technology-stack)
4. [Database](#database)
    1. [Schema Overview](#schema-overview)
    2. [Schema Creation Scripts](#schema-creation-scripts)
5. [GoServer](#goserver)
    1. [Folder Structure](#folder-structure)
    2. [Installation and Setup](#installation-and-setup)
    3. [Running the GoServer](#running-the-goserver)
    4. [Default Admin User](#default-admin-user)
    5. [API Endpoints](#api-endpoints)
6. [WPF Client Application](#wpf-client-application)
    1. [Architecture Overview](#architecture-overview)
    2. [Project Structure](#project-structure)
    3. [Installation and Setup](#installation-and-setup-1)
    4. [Running the Client](#running-the-client)
    5. [Key Screens and Functions](#key-screens-and-functions)
7. [Authentication and Roles](#authentication-and-roles)
8. [Troubleshooting](#troubleshooting)

---

## Features

1. **User Management**
    - **Admin** can create, update, and delete users (students, teachers, other admins).
    - **Teacher** can manage grades, view the list of students or subjects.
    - **Student** can view only their own grades.

2. **Subject Management**
    - Admins create, rename, and delete subjects.
    - Each subject can be associated with a teacher (optional in the schema).

3. **Grades Management**
    - Teachers and admins can add, update (if implemented), or delete grades for students in specific subjects.
    - Students can only read (view) their own grades.

4. **Authentication & Authorization**
    - **JWT**-based authentication and role-based authorization for all protected endpoints.
    - A default admin account is automatically created if no admin user is found.

5. **RESTful API**
    - All data exchanges occur over JSON-based endpoints (CRUD operations for users, subjects, and grades).

6. **Client Application (WPF)**
    - Desktop client that communicates with the GoServer via HTTP.
    - Provides a user-friendly interface for administrators, teachers, and students.
    - Implements **MVVM** for clear separation of concerns.

---

## System Architecture

This system comprises three primary layers:

1. **Database Layer (PostgreSQL)**
    - Stores all persistent data: users, subjects, and grades.
    - Exposed to the GoServer via standard SQL queries.

2. **Application Layer (GoServer)**
    - **Gin**-based HTTP server written in Go.
    - Serves RESTful endpoints for CRUD operations.
    - Manages role-based access and JWT authentication.

3. **Client Layer (WPF Application)**
    - A C#/.NET desktop application using WPF.
    - Communicates with GoServer’s endpoints (JSON over HTTP).
    - Implements an MVVM pattern for code organization.

---

## Technology Stack

- **Language & Frameworks**:
    - **Go (1.18+)** with **Gin** for the server
    - **C#/.NET (WPF)** for the client
- **Database**: PostgreSQL (version 9.6+ recommended)
- **Authentication**: JSON Web Tokens (JWT)
- **UI Patterns (Client)**: MVVM
- **HTTP Protocol**: RESTful JSON endpoints

---

## Database

### Schema Overview

Three tables are central to the system:

1. **Users**
    - Stores user account data (name, login, password hash, role).
    - Possible roles: `admin`, `teacher`, `student` (extendable as needed).

2. **Subjects**
    - Holds information about subjects/courses (title, optional teacher reference).

3. **Grades**
    - Links a **student** (user) with a **subject** and a numeric grade value.

**Relationships**:
- A single **user** can have many **grade** entries (if the user is a student).
- A single **subject** can have many **grade** entries.

### Schema Creation Scripts

Below is an example of creating the required tables in **PostgreSQL**. Adjust data types, indexes, constraints, or foreign keys as needed:

```sql
-- Create the Users table
CREATE TABLE IF NOT EXISTS Users (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    login TEXT NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    role TEXT NOT NULL
);

-- Create the Subjects table
CREATE TABLE IF NOT EXISTS Subjects (
    id SERIAL PRIMARY KEY,
    title TEXT NOT NULL,
    teacher_id INT
);

-- Create the Grades table
CREATE TABLE IF NOT EXISTS Grades (
    id SERIAL PRIMARY KEY,
    student_id INT NOT NULL,
    subject_id INT NOT NULL,
    grade INT NOT NULL,

    FOREIGN KEY (student_id) REFERENCES Users(id) ON DELETE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES Subjects(id) ON DELETE CASCADE
);
```

---

## GoServer

The **GoServer** project is a standalone web application that handles all server-side logic, including database connectivity, authentication, and authorization.

### Folder Structure

```
GoServer/
├── cmd/
│   └── main.go              // Application entry point (starts the HTTP server)
├── internal/
│   ├── db/
│   │   ├── db.go            // Establishes a DB connection
│   │   ├── user.go          // CRUD operations for Users
│   │   ├── subject.go       // CRUD operations for Subjects
│   │   ├── grade.go         // CRUD operations for Grades
│   ├── handlers/
│   │   ├── auth.go          // Login handler, token generation
│   │   ├── users.go         // HTTP handlers for user routes
│   │   ├── subjects.go      // HTTP handlers for subject routes
│   │   ├── grades.go        // HTTP handlers for grade routes
│   ├── middleware/
│   │   └── auth.go          // JWT middleware for protecting routes
│   ├── models/
│   │   ├── user.go          // Data model for User
│   │   ├── subject.go       // Data model for Subject
│   │   └── grade.go         // Data model for Grade
├── go.mod
└── go.sum
```

1. **`cmd/main.go`**
    - Initializes database connection, configures Gin routes, applies JWT middleware, and starts the web server.
2. **`internal/db/`**
    - Houses the code interfacing with PostgreSQL (CRUD queries).
3. **`internal/handlers/`**
    - Contains Gin-based route handlers for users, subjects, and grades.
4. **`internal/middleware/`**
    - JWT middleware that intercepts requests to ensure they have a valid token.
5. **`internal/models/`**
    - Plain Go structs representing `User`, `Subject`, and `Grade`.

### Installation and Setup

1. **Clone the repository**:
   ```bash
   git clone https://github.com/YourUsername/GoServer.git
   cd GoServer
   ```
2. **Fetch dependencies** (Go modules):
   ```bash
   go mod tidy
   ```
3. **Configure database connection**:
    - In `cmd/main.go` or wherever you open the DB connection, adjust the connection string parameters:
      ```go
      db.OpenDB("host=localhost port=5432 user=postgres password=postgres dbname=postgres sslmode=disable")
      ```
    - Make sure these match your actual PostgreSQL credentials.

4. **Create the database tables** (see [Schema Creation Scripts](#schema-creation-scripts)).

### Running the GoServer

Run the application from the project root:
```bash
go run cmd/main.go
```
By default, it listens on [http://localhost:8080](http://localhost:8080). Modify `r.Run(":8080")` in `main.go` if you need a different port.

### Default Admin User

- When the server starts, it checks if at least one **admin** user exists.
- If no admin is found, a default admin is created:
    - **Login**: `admin`
    - **Password**: `admin`
    - **Name**: `alex`
    - **Role**: `admin`

> **Security Note**: Change this default password immediately in production.

### API Endpoints

Below is a brief overview of key endpoints. All require an **Authorization: Bearer <token>** header except for `/login`.

1. **Authentication**
    - **`POST /login`**
        - Request:
          ```json
          { "login": "admin", "password": "admin" }
          ```
        - Response:
          ```json
          {
            "success": true,
            "token": "<JWT_TOKEN_HERE>",
            "user": {
              "id": 1,
              "name": "alex",
              "login": "admin",
              "role": "admin"
            }
          }
          ```

2. **Users**
    - **`GET /users`** – Lists all users (admin-only).
    - **`POST /users`** – Creates a new user.
    - **`PUT /users/:id`** – Updates user info.
    - **`DELETE /users/:id`** – Deletes a user (admin-only).

3. **Subjects**
    - **`GET /subjects`** – Lists all subjects.
    - **`POST /subjects`** – Creates a new subject (admin-only).
    - **`PUT /subjects/:id`** – Updates subject data (admin-only).
    - **`DELETE /subjects/:id`** – Deletes a subject (admin-only).

4. **Grades**
    - **`GET /grades?subject={id}`** – Lists all grades for a given subject.
    - **`GET /grades?student={id}`** – Lists all grades for a given student.
    - **`POST /grades`** – Creates a new grade record (teacher/admin).
    - **`DELETE /grades/:id`** – Deletes a grade (teacher/admin).

---

## WPF Client Application

A **WPF (Windows Presentation Foundation)** desktop application that consumes the GoServer’s RESTful API.

### Architecture Overview

The client follows the **MVVM** pattern:
1. **Models** – Simple C# classes representing `User`, `Subject`, and `Grade`.
2. **ViewModels** – Contain logic for handling data binding, user input, and interaction with the **ApiService**.
3. **Views** (XAML) – Define the UI, data-bound to their corresponding ViewModels.
4. **Services** – Encapsulate HTTP calls to the GoServer (e.g., `ApiService.cs`).

### Project Structure

```
Client/
├── Models/
│   ├── User.cs
│   ├── Subject.cs
│   └── Grade.cs
├── Services/
│   ├── ApiService.cs
│   ├── IApiService.cs
│   └── DTO.cs
├── ViewModels/
│   ├── LoginViewModel.cs
│   ├── MainViewModel.cs
│   ├── AdminPanelViewModel.cs
│   ├── SubjectsViewModel.cs
│   ├── AddSubjectDialogViewModel.cs
│   ├── AddUserDialogViewModel.cs
│   ├── AddGradeDialogViewModel.cs
│   ├── RenameSubjectDialogViewModel.cs
│   └── HomeViewModel.cs
├── Views/
│   ├── LoginView.xaml
│   ├── MainWindow.xaml
│   ├── AdminPanelView.xaml
│   ├── SubjectsView.xaml
│   ├── AddSubjectDialogView.xaml
│   ├── AddUserDialog.xaml
│   ├── GradeDialog.xaml
│   └── ...
├── RelayCommand.cs
├── GlobalState.cs
├── NavigationService.cs
└── App.xaml
```

### Installation and Setup

1. **Clone or Download** the client project into your local environment.
2. **Open in Visual Studio** (or another .NET-compatible IDE).
3. **Restore NuGet Packages** to ensure dependencies like `Newtonsoft.Json` are installed.
4. **Configure the API Endpoint** in `ApiService.cs` if your GoServer runs on a different host or port:
   ```csharp
   private readonly HttpClient _client = new HttpClient
   {
       BaseAddress = new Uri("http://localhost:8080")
   };
   ```
5. **Ensure the GoServer is running** (see [GoServer Setup](#installation-and-setup)).

### Running the Client

1. **Set the WPF project** as the **Startup Project** in Visual Studio.
2. Press **F5** or **Start** to run.
3. The **Login** window appears. Enter credentials (e.g., default admin: `admin` / `admin`).
4. On successful login, you are navigated to the **Main Window**.

### Key Screens and Functions

1. **Login Screen**
    - Prompts for `login` and `password`.
    - On success, saves JWT token in `GlobalState` and navigates to Main Window.

2. **Main Window**
    - Top-level interface with navigation to `Admin Panel`, `Subjects`, etc.
    - Displays user’s name and role.
    - Allows **logout**, which clears the token and returns to the Login screen.

3. **Admin Panel** (Admin only)
    - Lists all users (`GET /users`).
    - Allows adding new users, deleting existing users.

4. **Subjects View**
    - Lists subjects (all for admin/teacher; for students, can be filtered to only relevant subjects if desired).
    - Admin can add, rename, or delete subjects.
    - Teachers/Admins can see and manage grades for each subject (add/delete).
    - Students see only their own grades.

5. **Grades Management**
    - **Add Grade**: Teachers/Admin can create a new grade (`POST /grades`).
    - **Delete Grade**: Teachers/Admin can delete a grade (`DELETE /grades/:id`).
    - Students only read (view) their grades, no write operations.

---

## Authentication and Roles

- **JWT Tokens**: The GoServer generates a JWT upon login. The WPF client stores it and includes `Authorization: Bearer <token>` in every subsequent request.
- **Roles**:
    - **Admin**: Full CRUD on users, subjects, and grades.
    - **Teacher**: No access to user management; can read/create/delete grades; read subjects.
    - **Student**: Read-only access to personal grades.

---

## Troubleshooting

1. **Database Connection Issues**
    - Verify the PostgreSQL service is running and that the credentials in `OpenDB(...)` are correct.
    - Ensure the relevant tables have been created.
2. **Authentication Failures**
    - Make sure you are using the correct login credentials.
    - Confirm the WPF client is attaching the token in the request header (`Authorization`).
3. **Server Not Found / Connection Refused**
    - Verify the GoServer is running on the correct port (default: 8080).
    - Check firewall or network constraints if server is on a different machine.
4. **API Errors (404, 500, etc.)**
    - Confirm the endpoint paths are correct and the request body structure matches the server’s expected format.
5. **Default Admin Doesn’t Work**
    - If you changed the admin credentials in the code or already created an admin, the auto-creation step might be skipped. Manually insert an admin user into the `Users` table if needed.
