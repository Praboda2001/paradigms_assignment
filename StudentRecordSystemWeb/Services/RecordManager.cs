using Microsoft.EntityFrameworkCore;
using StudentRecordSystem.Data;
using StudentRecordSystem.Exceptions;
using StudentRecordSystem.Models;

namespace StudentRecordSystem.Services;

// Handles all the actual add/search/update/remove logic for student
// records, so the UI pages don't have to talk to the database directly.
public class RecordManager
{
	private readonly AppDbContext _context;
	private readonly AuthService _authService;

	// Fires whenever a new record gets added, so the admin dashboard can
	// refresh its list automatically without us having to call it directly.
	public event EventHandler<StudentRecord>? RecordAdded;

	public RecordManager(AppDbContext context, AuthService authService)
	{
		_context = context;
		_authService = authService;
	}

	// Gets every student record, sorted by Student ID.
	public async Task<List<StudentRecord>> GetAllRecordsAsync() =>
		await _context.StudentRecords
			.Include(r => r.Units)
			.OrderBy(r => r.StudentID)
			.ToListAsync();

	// Finds one record by Student ID, or throws if it doesn't exist.
	public async Task<StudentRecord> SearchRecordAsync(string studentId)
	{
		StudentRecord? record = await _context.StudentRecords
			.Include(r => r.Units)
			.FirstOrDefaultAsync(r => r.StudentID == studentId);

		return record ?? throw new StudentNotFoundException(studentId);
	}

	// Same as above but by internal database Id, and just returns null
	// instead of throwing — used when "not found" just means "add mode".
	public async Task<StudentRecord?> TryGetByIdAsync(int id) =>
		await _context.StudentRecords
			.Include(r => r.Units)
			.FirstOrDefaultAsync(r => r.Id == id);

	// Saves a brand-new student record and creates their login.
	public async Task AddRecordAsync(StudentRecord record)
	{
		bool exists = await _context.StudentRecords
			.AnyAsync(r => r.StudentID == record.StudentID);

		if (exists)
		{
			throw new DuplicateStudentException(record.StudentID);
		}

		record.CalculateWam();
		_context.StudentRecords.Add(record);
		await _context.SaveChangesAsync();

		// Only create the login once the record itself is safely saved.
		await _authService.CreateStudentAccountAsync(record);

		RecordAdded?.Invoke(this, record);
	}

	// Overwrites an existing record's details and units with new values.
	public async Task UpdateRecordAsync(StudentRecord updated)
	{
		StudentRecord existing = await SearchRecordAsync(updated.StudentID);

		existing.Name = updated.Name;
		existing.Program = updated.Program;
		existing.Status = updated.Status;

		// Simplest way to replace the whole units list — clear it out
		// and add back whatever was passed in.
		existing.Units.Clear();
		existing.Units.AddRange(updated.Units);
		existing.CalculateWam();

		await _context.SaveChangesAsync();
	}

	// Deletes a record (its units get deleted automatically too).
	public async Task RemoveRecordAsync(string studentId)
	{
		StudentRecord record = await SearchRecordAsync(studentId);
		_context.StudentRecords.Remove(record);
		await _context.SaveChangesAsync();
	}

	// Recalculates WAM for every student at once, spread across
	// multiple CPU cores instead of doing them one at a time.
	public async Task<int> RecalculateAllWamAsync()
	{
		List<StudentRecord> records = await GetAllRecordsAsync();

		await Task.Run(() =>
		{
			Parallel.ForEach(records, record => record.CalculateWam());
		});

		await _context.SaveChangesAsync();
		return records.Count;
	}
}