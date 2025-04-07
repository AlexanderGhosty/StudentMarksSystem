# GoServer

**GoServer** is a simple web application built using the [Go programming language](https://go.dev/), the [Gin](https://github.com/gin-gonic/gin) web framework, and a PostgreSQL database. This application demonstrates how to implement basic user management with role-based permissions, as well as managing subjects and grades for students. It uses JSON Web Tokens (JWT) for authentication and authorization.

---

## Table of Contents

1. [Features](#features)
2. [Project Structure](#project-structure)
3. [Requirements](#requirements)
4. [Installation and Setup](#installation-and-setup)
5. [Database Schema](#database-schema)
6. [Running the Application](#running-the-application)
7. [Default Admin User](#default-admin-user)
8. [API Endpoints](#api-endpoints)
    - [Authentication](#authentication)
    - [Users](#users)
    - [Subjects](#subjects)
    - [Grades](#grades)

---

## Features

- **JWT-Based Authentication**: Secure login mechanism using JSON Web Tokens.
- **Role-Based Authorization**: Users can have roles such as **admin**, **teacher**, or other roles, with different levels of access.
- **User Management**: Create, retrieve, update, and delete users.
- **Subjects Management**: Create, list, update, and delete school subjects.
- **Grades Management**: Record and manage students’ grades for specific subjects.
- **RESTful Endpoints**: All operations exposed via JSON-based HTTP endpoints.

---

## Project Structure

```
GoServer/
├── cmd/
│   └──main.go              // Application entry point
├── internal/
│   ├── db/
│   │   ├── db.go          // Establishes a DB connection
│   │   ├── user.go        // CRUD operations for users in the database
│   │   ├── subject.go     // CRUD operations for subjects in the database
│   │   ├── grade.go       // CRUD operations for grades in the database
│   ├── handlers/
│   │   ├── auth.go        // Login handler, token generation
│   │   ├── users.go       // HTTP handlers for user routes
│   │   ├── subjects.go    // HTTP handlers for subject routes
│   │   ├── grades.go      // HTTP handlers for grade routes
│   ├── middleware/
│   │   └── auth.go        // JWT middleware for route protection
│   ├── models/
│   │   ├── user.go        // User struct
│   │   ├── subject.go     // Subject struct
│   │   ├── grade.go       // Grade struct               
├── go.mod                 // Go module file
└── go.sum                 // Go dependencies lock file
```

**Key components**:
1. **`cmd/main.go`**: Sets up the server, routes, and middleware.
2. **`internal/db/`**: Database logic (connecting to the database, CRUD queries).
3. **`internal/handlers/`**: Gin-based HTTP handlers for different resources (users, subjects, grades).
4. **`internal/middleware/`**: Authentication middleware for protecting routes.
5. **`internal/models/`**: Data models representing tables (Users, Subjects, Grades).

---

## Requirements

- **Go** 1.18 or newer
- **PostgreSQL** 9.6+ (adjust connection parameters as needed)
- Git, if you wish to clone the repository directly

---

## Installation and Setup

1. **Clone the repository** (or copy the files into a new Go module):
   ```bash
   git clone https://github.com/YourUsername/GoServer.git
   cd GoServer
   ```

2. **Initialize Go modules** (if needed):
   ```bash
   go mod tidy
   ```

3. **Configure your database connection**:
    - In `main.go`, inside `db.OpenDB(...)`, there is a connection string:
      ```go
      db.OpenDB("host=localhost port=5432 user=postgres password=postgres dbname=postgres sslmode=disable")
      ```
    - Adjust these parameters to match your PostgreSQL setup (host, port, user, password, dbname, sslmode).

4. **Create the necessary database tables**:
    - The application expects the following tables:
        - **Users**: Stores user details (`id`, `name`, `login`, `password_hash`, `role`).
        - **Subjects**: Stores subject details (`id`, `title`, `teacher_id`).
        - **Grades**: Stores grades (`id`, `student_id`, `subject_id`, `grade`).

   Create these tables in your PostgreSQL database manually or via migrations, for example:
   ```sql
   CREATE TABLE IF NOT EXISTS Users (
       id SERIAL PRIMARY KEY,
       name VARCHAR(100) NOT NULL,
       login VARCHAR(100) NOT NULL UNIQUE,
       password_hash VARCHAR(255) NOT NULL,
       role VARCHAR(50) NOT NULL
   );

   CREATE TABLE IF NOT EXISTS Subjects (
       id SERIAL PRIMARY KEY,
       title VARCHAR(100) NOT NULL,
       teacher_id INT
   );

   CREATE TABLE IF NOT EXISTS Grades (
       id SERIAL PRIMARY KEY,
       student_id INT NOT NULL,
       subject_id INT NOT NULL,
       grade INT NOT NULL
   );
   ```

---

## Database Schema

A brief overview of the tables:

- **Users**
    - `id` (PK)
    - `name` (string)
    - `login` (string, unique)
    - `password_hash` (string, hashed password)
    - `role` (string, e.g., "admin", "teacher", "student", etc.)

- **Subjects**
    - `id` (PK)
    - `title` (string, subject name)
    - `teacher_id` (int, references a User ID – if needed)

- **Grades**
    - `id` (PK)
    - `student_id` (int, references a User ID for the student)
    - `subject_id` (int, references a Subject ID)
    - `grade` (int, numeric grade value)

---

## Running the Application

Use Go’s built-in tool to run:

```bash
go run main.go
```

By default, the server starts at **`http://localhost:8080`**. You can modify the `r.Run(":8080")` line in `main.go` if you need a different port.

---

## Default Admin User

When the application starts, it checks if there is at least one user with the role `admin` in the **Users** table.
- If **no** admin user is found, the application automatically creates a default admin with:
    - **Name**: `alex`
    - **Login**: `admin`
    - **Password**: `admin` (stored in the database as a hashed value)
    - **Role**: `admin`

Use this account to log in initially and create additional users.
> **Important**: Change the admin password as soon as possible for security.

---

## API Endpoints

All routes (except the login route) are protected by JWT-based authentication. You must include an `Authorization` header with the format `Bearer <token>` to access them once you have logged in.

### Authentication

- **`POST /login`**

  Request body:
  ```json
  {
    "login": "admin",
    "password": "admin"
  }
  ```
  Successful response:
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
  Use the returned `token` in the `Authorization` header for subsequent requests:  
  `Authorization: Bearer <JWT_TOKEN_HERE>`

---

### Users

**All user endpoints** are grouped under `authorized` routes protected by the JWT middleware. Only users with valid tokens can access them, and certain actions (like `DELETE /users/:id`) may require the **admin** role.

1. **`GET /users`**  
   Retrieves a list of all users.  
   Example response:
   ```json
   [
     {
       "id": 1,
       "name": "alex",
       "login": "admin",
       "role": "admin"
     },
     ...
   ]
   ```

2. **`POST /users`**  
   Creates a new user. Request body:
   ```json
   {
     "name": "John Doe",
     "login": "john",
     "passwordHash": "plaintext123",
     "role": "teacher"
   }
   ```
    - The server automatically hashes the password before storing it.
    - Returns the created user with `id`.

3. **`PUT /users/:id`**  
   Updates an existing user. Request body can include any subset of:
   ```json
   {
     "name": "John Updated",
     "login": "john_new",
     "role": "admin",
     "password": "newPassword123"
   }
   ```
    - If `"password"` is present, it is hashed again before saving.

4. **`DELETE /users/:id`**  
   Deletes the user with the specified ID.
    - Only users with the **admin** role can delete other users.

---

### Subjects

These endpoints let you manage subjects taught in the system.

1. **`GET /subjects`**  
   Returns a list of subjects:
   ```json
   [
     {
       "id": 1,
       "title": "Mathematics",
       "teacher_id": 2
     },
     ...
   ]
   ```

2. **`POST /subjects`**  
   Creates a new subject. Example request:
   ```json
   {
     "title": "Physics",
     "teacher_id": 3
   }
   ```
   Returns the created subject with its new `id`.

3. **`PUT /subjects/:id`**  
   Updates the subject with new data. For example:
   ```json
   {
     "title": "Advanced Physics",
     "teacher_id": 4
   }
   ```
   Returns success if updated.

4. **`DELETE /subjects/:id`**  
   Deletes the subject with the given `id`.

---

### Grades

These endpoints let you store and view student grades for specific subjects.

1. **`GET /grades`**  
   Returns grades based on query parameters:
    - **`GET /grades?subject=1`**: Returns all grades for **subject ID = 1**.
    - **`GET /grades?student=2`**: Returns all grades for **student ID = 2**.
    - **No parameters**: Returns an empty array by default in this implementation.

2. **`POST /grades`**  
   Creates a new grade record. Request body:
   ```json
   {
     "student_id": 2,
     "subject_id": 1,
     "grade": 90
   }
   ```
   Returns success on creation.

3. **`DELETE /grades/:id`**  
   Deletes the grade by its `id`. Only **admin** or **teacher** roles can delete a grade.
