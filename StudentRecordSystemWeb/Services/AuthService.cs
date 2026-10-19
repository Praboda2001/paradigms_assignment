// Student Name: P.H. Thushan Prabodha
// Student ID: 10705662
// Date: 23/09/2026

using Microsoft.EntityFrameworkCore;
using StudentRecordSystem.Data;
using StudentRecordSystem.Exceptions;
using StudentRecordSystem.Models;

namespace StudentRecordSystem.Services;

// This handles logging in and creating login accounts.
public class AuthService
{
    // The database connection is passed in by dependency injection.
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    // This makes sure the database exists and creates a default admin login
    // the very first time the app runs.
    public async Task InitialiseAsync()
    {
        await _context.Database.EnsureCreatedAsync();

        bool hasAdmin = await _context.Accounts.AnyAsync(a => a.Role == AccountRole.Admin);
        if (!hasAdmin)
        {
            _context.Accounts.Add(new Account
            {
                Username = "admin",
                PasswordHash = PasswordHasher.Hash("admin123"),
                Role = AccountRole.Admin
            });
            await _context.SaveChangesAsync();
        }
    }

    // This checks a username and password against the database and returns
    // the matching Admin or Student. It throws when the login is wrong instead
    // of returning null, so the page only needs one place to catch the problem.
    public async Task<User> LoginAsync(string username, string password)
    {
        // FirstOrDefaultAsync returns null when no account matches, which is
        // why the variable is declared as Account? (nullable).
        Account? account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Username == username);

        // && and || short-circuit, so Verify is never called for a missing account.
        // One generic exception is used for both failures so the message does
        // not reveal whether the username exists.
        if (account is null || !PasswordHasher.Verify(password, account.PasswordHash))
        {
            throw new InvalidLoginException();
        }

        // A switch expression turns the saved Account row into the right kind of User.
        return account.Role switch
        {
            AccountRole.Admin => new Admin { AccountId = account.Id, Username = account.Username },
            AccountRole.Student => new Student
            {
                AccountId = account.Id,
                Username = account.Username,
                // ?? throw fails loudly if a student account has no linked record.
                StudentRecordId = account.StudentRecordId
                    ?? throw new InvalidOperationException("Student account is missing its linked record.")
            },
            _ => throw new InvalidOperationException("Unknown account role.")
        };
    }

    // This tells the caller whether a login name is already taken, so a new
    // student cannot be given the same name as an existing account.
    public async Task<bool> UsernameExistsAsync(string username) =>
        await _context.Accounts.AnyAsync(a => a.Username == username);

    // This creates a login for a new student. Their Student ID is used as
    // both their username and their starting password.
    public async Task CreateStudentAccountAsync(StudentRecord record)
    {
        var account = new Account
        {
            Username = record.StudentID,
            PasswordHash = PasswordHasher.Hash(record.StudentID),
            Role = AccountRole.Student,
            StudentRecordId = record.Id
        };
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
    }
}
