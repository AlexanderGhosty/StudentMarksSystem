# Database Documentation

## Table of Contents

1. [Overview](#overview)
2. [Database Schema](#database-schema)
    - [Users Table](#users-table)
    - [Subjects Table](#subjects-table)
    - [Grades Table](#grades-table)
3. [Relationships](#relationships)
4. [SQL Schema Creation Scripts](#sql-schema-creation-scripts)
5. [Go Code Overview](#go-code-overview)
    - [User Management (`user.go`)](#user-management-usergo)
    - [Subject Management (`subject.go`)](#subject-management-subjectgo)
    - [Grade Management (`grade.go`)](#grade-management-gradego)

---

## Overview

The system manages three primary entities:

1. **Users**: Represents both students and teachers.
2. **Subjects**: Represents courses or subjects.
3. **Grades**: Represents a record of a student’s grade in a particular subject.

The Go code (in the `db` package) contains helper functions that communicate with a PostgreSQL database to perform CRUD operations on these entities.

---

## Database Schema

### Users Table

| Column         | Type    | Description                                          |
|----------------|---------|------------------------------------------------------|
| **id**         | SERIAL  | Primary key (auto-increment).                        |
| **name**       | TEXT    | The name of the user (student or teacher).           |
| **login**      | TEXT    | Unique login identifier.                             |
| **password_hash** | TEXT | Hashed password for authentication.                  |
| **role**       | TEXT    | Role of the user (e.g., `student` or `teacher` or `admin`). |

### Subjects Table

| Column      | Type   | Description                                           |
|-------------|--------|-------------------------------------------------------|
| **id**      | SERIAL | Primary key (auto-increment).                         |
| **title**   | TEXT   | The name or title of the subject.                     |


### Grades Table

| Column         | Type   | Description                                                         |
|----------------|--------|---------------------------------------------------------------------|
| **id**         | SERIAL | Primary key (auto-increment).                                       |
| **student_id** | INT    | References a user’s ID who is a student (foreign key).              |
| **subject_id** | INT    | References a subject’s ID (foreign key).                            |
| **grade**      | INT    | Numeric value representing the student’s grade in that subject.      |

---

## Relationships

1. **Users**  
   Each user can have either a **teacher** or **student** or **admin** role.

2. **Subjects**  
   
3. **Grades**  
   Each grade record ties a **student** (from the **Users** table) to a **subject** (from the **Subjects** table) with a specific grade value.

Hence:

- **One** subject can be associated with **many** grade entries, each belonging to a student (1-to-many relationship).
- **One** user with the student role can have **many** grades across different subjects (1-to-many relationship).

---

## SQL Schema Creation Scripts

Below is an example of how you can create the schema in PostgreSQL. Adjust data types and constraints according to your project’s requirements.

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
    title TEXT NOT NULL
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

## Go Code Overview

The Go files (`grade.go`, `subject.go`, `user.go`) reside in the `db` package. Each file corresponds to CRUD operations on a specific table.

### User Management (`user.go`)

Methods include:

1. **GetUsers**
   ```go
   func GetUsers(db *sql.DB) ([]models.User, error)
   ```
    - Retrieves all user records from the `Users` table.
    - Returns a slice of `models.User`.

2. **CreateUser**
   ```go
   func CreateUser(db *sql.DB, user models.User, passwordHash string) (int, error)
   ```
    - Inserts a new user into the `Users` table.
    - Returns the newly generated user ID.

3. **DeleteUser**
   ```go
   func DeleteUser(db *sql.DB, userId int) error
   ```
    - Deletes a user by `id`.
    - Returns an error if the user doesn’t exist or the operation fails.

4. **UpdateUser**
   ```go
   func UpdateUser(db *sql.DB, user models.User) error
   ```
    - Updates selected columns of a user record if they are not empty in the `models.User` struct.
    - Dynamically builds the SQL query based on which fields are set.

---

### Subject Management (`subject.go`)

Methods include:

1. **GetSubjects**
   ```go
   func GetSubjects(db *sql.DB) ([]models.Subject, error)
   ```
    - Retrieves all subject records from the `Subjects` table.
    - Returns a slice of `models.Subject`.

2. **CreateSubject**
   ```go
   func CreateSubject(db *sql.DB, subj models.Subject) (int, error)
   ```
    - Inserts a new subject into the `Subjects` table.
    - Returns the new subject’s ID.

3. **DeleteSubject**
   ```go
   func DeleteSubject(db *sql.DB, subjectId int) error
   ```
    - Deletes a subject by `id`.
    - Returns an error if the operation fails.

4. **UpdateSubject**
   ```go
   func UpdateSubject(db *sql.DB, subj models.Subject) error
   ```
    - Updates title and teacher ID for a subject.
    - Returns an error if the operation fails.

---

### Grade Management (`grade.go`)

Methods include:

1. **AddGrade**
   ```go
   func AddGrade(db *sql.DB, grade models.Grade) error
   ```
    - Inserts a new grade record into the `Grades` table.

2. **GetGradesBySubject**
   ```go
   func GetGradesBySubject(db *sql.DB, subjectId int) ([]models.Grade, error)
   ```
    - Retrieves all grades for a given subject by `subjectId`.
    - Joins with the `Users` table to get the student’s name.

3. **GetGradesByStudent**
   ```go
   func GetGradesByStudent(db *sql.DB, studentId int) ([]models.Grade, error)
   ```
    - Retrieves all grades for a specific student by `studentId`.
    - Joins with the `Users` table to get the student’s name.

4. **DeleteGrade**
   ```go
   func DeleteGrade(db *sql.DB, gradeId int) error
   ```
    - Deletes a grade record by `id`.
    - Returns an error if the record does not exist or the operation fails.

---
