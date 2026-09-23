using Microsoft.EntityFrameworkCore;
using StudentRecordSystem.Data;
using StudentRecordSystem.Exceptions;
using StudentRecordSystem.Models;

namespace StudentRecordSystem.Services;

// Handles logging in and creating new accounts.
public class AuthService
{
	private readonly AppDbContext _context;

	public AuthService(AppDbContext context)
	{
		_context = context;
	}

	// Makes sure the database exists and creates a default admin login
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

	// Checks a username/password against the database and returns the
	// matching Admin or Student. Throws if the login is wrong instead
	// of returning null, so the UI only needs one place to catch it.
	public async Task<User> LoginAsync(string username, string password)
	{
		Account? account = await _context.Accounts
			.FirstOrDefaultAsync(a => a.Username == username);

		if (account is null || !PasswordHasher.Verify(password, account.PasswordHash))
		{
			throw new InvalidLoginException();
		}

		// Turn the saved Account row into the right kind of User object.
		return account.Role switch
		{
			AccountRole.Admin => new Admin { AccountId = account.Id, Username = account.Username },
			AccountRole.Student => new Student
			{
				AccountId = account.Id,
				Username = account.Username,
				StudentRecordId = account.StudentRecordId
					?? throw new InvalidOperationException("Student account is missing its linked record.")
			},
			_ => throw new InvalidOperationException("Unknown account role.")
		};
	}

	// Creates a login for a new student — their Student ID doubles as
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