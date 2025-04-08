
# Client Application (WPF)

This **Client** is a WPF application that allows administrators, teachers, and students to interact with the **GoServer** . The client communicates with the server over HTTP (JSON endpoints) and uses JWT-based authentication to manage user sessions and roles.

## Table of Contents
1. [Features](#features)
2. [Technologies Used](#technologies-used)
3. [Prerequisites](#prerequisites)
4. [Architecture Overview](#architecture-overview)
5. [Project Structure](#project-structure)
6. [Installation and Setup](#installation-and-setup)
7. [Running the Client](#running-the-client)
8. [Authentication and Roles](#authentication-and-roles)
9. [Key Screens and Functions](#key-screens-and-functions)
   - [Login Screen](#login-screen)
   - [Main Window](#main-window)
   - [Admin Panel](#admin-panel)
   - [Subjects and Grades View](#subjects-and-grades-view)
10. [API Interaction](#api-interaction)
11. [Troubleshooting](#troubleshooting)

---

## Features

- **Role-Based UI** – Depending on whether you are an admin, teacher, or student, different sets of features are enabled.  
- **User Management** – Admins can add, delete, and list all users.  
- **Subjects Management** – Admins can create and rename subjects; the application can also list subjects for all roles.  
- **Grades Management** – Teachers/Admins can add and delete student grades. Students can view only their own grades.  
- **RESTful Communication** – Uses `HttpClient` to call the Go server’s RESTful API at `http://localhost:8080` by default.  
- **MVVM Pattern** – Separation of concerns between Views, ViewModels, and Models.  
- **JWT Authentication** – Users must log in to receive a JWT token, which is used for authorized endpoints.

---

## Technologies Used

- **.NET (C#)** with **WPF** for the desktop client.
- **Newtonsoft.Json** for JSON serialization and deserialization.
- **MVVM** design pattern.
- **RelayCommand** for WPF command bindings.
- **HttpClient** for HTTP requests to the server.

---

## Prerequisites

1. **.NET Desktop Runtime / SDK**  
   - Ensure you have the .NET 6 (or higher) SDK or a version compatible with your WPF project.
2. **GoServer Running Locally**  
   - The Go-based server and a PostgreSQL instance (see [GoServer README.md] and [Database README.md]) must be running so the client can communicate. By default, the client expects the server at `http://localhost:8080`.
3. **Administrator or Student Accounts**  
   - If you are running from scratch, the GoServer automatically creates a default admin user with login `admin`, password `admin`.

---

## Architecture Overview

The client follows the **MVVM** pattern:

- **Models** (`Client.Models`)  
  Plain C# classes (DTOs) representing `User`, `Subject`, `Grade`, etc.
- **ViewModels** (`Client.ViewModels`)  
  Contain application logic and state for each screen (e.g., `LoginViewModel`, `MainViewModel`, `SubjectsViewModel`).
- **Views** (`Client.Views`)  
  XAML files with minimal code-behind; data-bound to ViewModels.
- **Services** (`Client.Services`)  
  Implementation of `IApiService`, which communicates with the server’s endpoints.  
- **GlobalState**  
  Holds the currently logged-in user and the JWT token.

---

## Project Structure

Below is an excerpt of the relevant folders and files:

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

---

## Installation and Setup

1. **Clone or Download** this client repository into your local development environment.

2. **Open in Visual Studio / JetBrains Rider / VS Code**  
   - Navigate to the folder containing the `.csproj` or `.sln` file.
   - Open the `.sln` in your preferred IDE.

3. **Check NuGet Packages**  
   - Make sure all required NuGet packages (e.g., `Newtonsoft.Json`) are installed.  
   - Usually, you can right-click on the solution and select **Restore NuGet Packages** in Visual Studio.

4. **Configure the API Endpoint (Optional)**  
   - By default, `ApiService.cs` sets `http://localhost:8080` as the base address.  
   - If your server is on a different address or port, update `_client.BaseAddress` in `ApiService.cs`.

5. **Ensure Server is Running**  
   - Start your GoServer (see [GoServer README.md]) so that the client can send requests (login, CRUD operations, etc.).

---

## Running the Client

1. **Set the Startup Project**  
   - In Visual Studio, right-click on the client project and select **Set as Startup Project** (if you have multiple projects in the solution).

2. **Compile and Run**  
   - Press **F5** or select **Start** from your IDE.  
   - The **LoginView** window will appear.

3. **Login**  
   - Use the default admin credentials (`admin` / `admin`) if you have not created other accounts.  
   - On successful login, the client will open the **Main Window**.

---

## Authentication and Roles

Once a user logs in, the server returns a **JWT** token. The client stores this token in `GlobalState.Token` and automatically attaches it to each subsequent request in the `Authorization: Bearer <JWT_TOKEN>` header.  

**Role-based behavior**:
- **Admin** – Full access to create, list, update, and delete users, subjects, and grades.
- **Teacher** – Cannot manage users but can create or remove grades for their students and view all subjects.
- **Student** – Can view only their own grades for each subject. Cannot create or remove subjects/grades.

---

## Key Screens and Functions

### Login Screen
- **File**: `LoginView.xaml` / `LoginViewModel.cs`
- **Functionality**:  
  - Prompts user for **Login** and **Password**.  
  - Sends a `POST` to `/login`.  
  - On success, stores token and user info, then navigates to the **Main Window**.  
  - On failure, displays an error message.

### Main Window
- **File**: `MainWindow.xaml` / `MainViewModel.cs`
- **Functionality**:  
  - Displays high-level navigation options (e.g., **Home**, **Admin Panel**, **Subjects**).  
  - Houses a placeholder content area that swaps between different **ViewModels** (via `CurrentViewModel` property).  
  - Allows the user to **logout**, which clears the token and user info, then navigates back to the **Login** window.

### Admin Panel
- **File**: `AdminPanelView.xaml` / `AdminPanelViewModel.cs`
- **Functionality** (Admin-Only):
  - **List All Users** by calling `GET /users`.
  - **Add User** – Opens a dialog (`AddUserDialogView.xaml`) to enter new user info, then calls `POST /users`.
  - **Delete User** – Removes the selected user with `DELETE /users/{id}`.

### Subjects and Grades View
- **File**: `SubjectsView.xaml` / `SubjectsViewModel.cs`
- **Functionality**:  
  - **List Subjects**:
    - Admin/Teacher: All subjects (`GET /subjects`).  
    - Student: Only subjects where they have grades.  
  - **Add Subject** (Admin Only): Opens `AddSubjectDialogView` → calls `POST /subjects`.
  - **Delete Subject** (Admin Only): `DELETE /subjects/{id}`.
  - **Rename Subject** (Admin Only): `PUT /subjects/{id}` with a new title.
  - **Grades**:
    - Displays all grades for a selected subject if Admin/Teacher.  
    - If a student, shows only their own grades for that subject.
    - **Add Grade** (Teacher/Admin Only): Opens `GradeDialog.xaml` to select a student and grade → calls `POST /grades`.
    - **Delete Grade** (Teacher/Admin Only): `DELETE /grades/{id}`.

---

## API Interaction

The core interaction happens in **`ApiService.cs`** implementing **`IApiService`**:

- **`LoginAsync`** – Sends credentials to `/login`, receives a JWT token on success.
- **`GetAllUsersAsync`** – `GET /users`
- **`CreateUserAsync`** – `POST /users`
- **`DeleteUserAsync`** – `DELETE /users/{id}`
- **`GetAllSubjectsAsync`** – `GET /subjects`
- **`CreateSubjectAsync`** – `POST /subjects`
- **`DeleteSubjectAsync`** – `DELETE /subjects/{id}`
- **`UpdateSubjectAsync`** – `PUT /subjects/{id}`
- **`GetGradesBySubjectAsync`** – `GET /grades?subject={subjectId}`
- **`GetGradesByStudentAsync`** – `GET /grades?student={studentId}`
- **`AddGradeAsync`** – `POST /grades`
- **`DeleteGradeAsync`** – `DELETE /grades/{id}`

The client attaches the token via:
```csharp
_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
```

---

## Troubleshooting

- **Cannot connect to server**:  
  Make sure the GoServer is running on `http://localhost:8080` or update `ApiService.cs` with the correct URL if using a different host/port.
- **Authentication errors**:  
  Double-check login credentials and ensure the token is present in `GlobalState.Token`.
- **CORS or Firewall issues**:  
  Typically not an issue for localhost calls, but if you host the server on a different machine, ensure connectivity is allowed.
- **404 / 500 Errors**:  
  Verify the server’s endpoints are correct and that the database has been set up according to the [Database README.md].
