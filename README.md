# Student Record System (CSP3341 Assignment 2)

A Blazor Server web application written in C# 14 on .NET 10. It stores student records in SQLite through Entity Framework Core and has separate Admin and Student access.

## How to run

1. Install the .NET 10 SDK.
2. Open a terminal in the `StudentRecordSystemWeb` folder.
3. Run `dotnet run`.
4. Open the address shown in the terminal (http://localhost:5140).

The database file `studentrecords.db3` is created automatically in the build output folder the first time the app starts. Running `dotnet clean` deletes it.

## Demo logins

| Role    | Username                      | Password                    |
|---------|-------------------------------|-----------------------------|
| Admin   | `admin`                       | `admin123`                  |
| Student | the Student ID the admin adds | the same Student ID         |

## Features

- On the first run five demo students (STU1001 to STU1005) are added so the ranking table has data. Each can log in with their Student ID as both the username and the password.
- Admin: add, edit, search and remove student records, see each student's class rank and a WAM bar, and recalculate every WAM in parallel. Removing a record asks for confirmation first.
- Student: sign in and see only their own record and units.
- Passwords are stored as SHA-256 hashes. A salted slow hash such as Argon2id would be stronger and is listed as a limitation in the report.
- Signing in lasts for one browser connection, so refreshing the page returns to the login screen.

## Project layout

- `Models` - `User` (abstract), `Admin`, `Student`, `StudentRecord`, `UnitEnrollment`, `Account`
- `Exceptions` - `DuplicateStudentException`, `StudentNotFoundException`, `InvalidLoginException`
- `Data` - `AppDbContext` (EF Core and SQLite)
- `Services` - `AuthService`, `RecordManager`, `SessionService`, `PasswordHasher`
- `Components/Pages` - `Login`, `AdminDashboard`, `StudentDashboard`, `StudentEdit`
