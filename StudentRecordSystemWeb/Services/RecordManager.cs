// Student Name: P.H. Thushan Prabodha
// Student ID: 10705662
// Date: 23/09/2026

using Microsoft.EntityFrameworkCore;
using StudentRecordSystem.Data;
using StudentRecordSystem.Exceptions;
using StudentRecordSystem.Models;

namespace StudentRecordSystem.Services;

// This handles all the add, search, update and remove logic for student
// records so the pages never talk to the database directly.
public class RecordManager
{
    // These are passed in by dependency injection (see Program.cs).
    private readonly AppDbContext _context;
    private readonly AuthService _authService;

    // This event is raised whenever a new record is added. Any page can
    // subscribe to it with += and unsubscribe with -=. The ? means the event
    // can be null when nothing has subscribed.
    public event EventHandler<StudentRecord>? RecordAdded;

    public RecordManager(AppDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    // This gets every student record sorted by Student ID. The LINQ chain is
    // only a description of the query until ToListAsync runs it against the
    // database (deferred execution). Include loads each student's units too.
    public async Task<List<StudentRecord>> GetAllRecordsAsync() =>
        await _context.StudentRecords
            .Include(r => r.Units)
            .OrderBy(r => r.StudentID)
            .ToListAsync();

    // This finds one record by Student ID or throws when it does not exist.
    public async Task<StudentRecord> SearchRecordAsync(string studentId)
    {
        StudentRecord? record = await _context.StudentRecords
            .Include(r => r.Units)
            .FirstOrDefaultAsync(r => r.StudentID == studentId);

        // ?? throw replaces a four-line if and throw check.
        return record ?? throw new StudentNotFoundException(studentId);
    }

    // This finds one record by its internal database Id and returns null
    // instead of throwing. It is used when "not found" just means add mode.
    public async Task<StudentRecord?> TryGetByIdAsync(int id) =>
        await _context.StudentRecords
            .Include(r => r.Units)
            .FirstOrDefaultAsync(r => r.Id == id);

    // This saves a new student record, creates their login and then raises
    // the RecordAdded event.
    public async Task AddRecordAsync(StudentRecord record)
    {
        // The Student ID is also the login name, so it must be free in both
        // the records table and the accounts table.
        bool exists = await _context.StudentRecords
            .AnyAsync(r => r.StudentID == record.StudentID)
            || await _authService.UsernameExistsAsync(record.StudentID);

        if (exists)
        {
            throw new DuplicateStudentException(record.StudentID);
        }

        record.CalculateWam();
        _context.StudentRecords.Add(record);
        await _context.SaveChangesAsync();

        // The login is created only after the record itself is saved.
        await _authService.CreateStudentAccountAsync(record);

        // ?. only calls Invoke when at least one handler is subscribed.
        RecordAdded?.Invoke(this, record);
    }

    // This overwrites an existing record's details and units with new values.
    public async Task UpdateRecordAsync(StudentRecord updated)
    {
        StudentRecord existing = await SearchRecordAsync(updated.StudentID);

        existing.Name = updated.Name;
        existing.Program = updated.Program;
        existing.Status = updated.Status;

        // The simplest way to replace the whole units list is to clear it
        // and add back whatever was passed in.
        existing.Units.Clear();
        existing.Units.AddRange(updated.Units);
        existing.CalculateWam();

        await _context.SaveChangesAsync();
    }

    // This deletes a record. Its units are deleted automatically because the
    // relationship is set to cascade in AppDbContext.
    public async Task RemoveRecordAsync(string studentId)
    {
        StudentRecord record = await SearchRecordAsync(studentId);

        // The login is a separate table, so it has to be removed here too.
        // Otherwise the old Student ID could not be added again.
        List<Account> accounts = await _context.Accounts
            .Where(a => a.StudentRecordId == record.Id)
            .ToListAsync();
        _context.Accounts.RemoveRange(accounts);

        _context.StudentRecords.Remove(record);
        await _context.SaveChangesAsync();
    }

    // This works out each student's class rank by WAM (1 = highest). Students
    // with the same WAM share a rank and the next rank is skipped, like a
    // normal leaderboard (1, 2, 2, 4). The result maps record Id to rank.
    public Dictionary<int, int> CalculateRanks(List<StudentRecord> records)
    {
        // Sort the highest WAM first using a lambda expression.
        var ordered = records.OrderByDescending(r => r.Wam).ToList();
        var ranks = new Dictionary<int, int>();

        int rank = 0;
        // double? is a nullable value type. It is null until the first record
        // has been compared.
        double? previousWam = null;

        for (int i = 0; i < ordered.Count; i++)
        {
            // The rank only moves on when the WAM changes, so ties share a rank.
            if (ordered[i].Wam != previousWam)
            {
                rank = i + 1;
                previousWam = ordered[i].Wam;
            }
            ranks[ordered[i].Id] = rank;
        }

        return ranks;
    }

    // This recalculates WAM for every student at once, spread across several
    // CPU cores instead of one record at a time.
    public async Task<int> RecalculateAllWamAsync()
    {
        // Database work is I/O-bound so it is awaited.
        List<StudentRecord> records = await GetAllRecordsAsync();

        // The calculation is CPU-bound. Parallel.ForEach spreads it over thread
        // pool threads and Task.Run keeps the calling thread free. This is safe
        // because each iteration changes only its own record and the
        // DbContext is not used inside the loop.
        await Task.Run(() =>
        {
            Parallel.ForEach(records, record => record.CalculateWam());
        });

        await _context.SaveChangesAsync();
        return records.Count;
    }

    // This adds a few demo students the first time the app runs so the ranking
    // table and WAM bar have data to show. It does nothing when records exist.
    public async Task SeedDemoDataAsync()
    {
        if (await _context.StudentRecords.AnyAsync())
        {
            return;
        }

        // Two students share a WAM of 85 so the table shows a tied rank.
        var demoStudents = new[]
        {
            CreateDemoStudent("STU1001", "Nimali Perera", 92, 88),
            CreateDemoStudent("STU1002", "Kasun Silva", 85, 85),
            CreateDemoStudent("STU1003", "Dilini Fernando", 78, 92),
            CreateDemoStudent("STU1004", "Ruwan Jayasinghe", 70, 66),
            CreateDemoStudent("STU1005", "Ishara Weerasinghe", 55, 61)
        };

        foreach (StudentRecord student in demoStudents)
        {
            await AddRecordAsync(student);
        }
    }

    // This builds one demo student with two units. The collection initialiser
    // adds the units to the Units list that the class already creates.
    private static StudentRecord CreateDemoStudent(string studentId, string name, double markOne, double markTwo) =>
        new()
        {
            StudentID = studentId,
            Name = name,
            Program = "Bachelor of Information Technology",
            Status = "Active",
            Units =
            {
                new UnitEnrollment { UnitCode = "CSP3341", UnitName = "Programming Languages and Paradigms", Semester = "2026 S2", Grade = GradeFor(markOne), Mark = markOne },
                new UnitEnrollment { UnitCode = "CSI3344", UnitName = "Distributed Systems", Semester = "2026 S2", Grade = GradeFor(markTwo), Mark = markTwo }
            }
        };

    // This turns a mark into a grade letter using relational patterns in a
    // switch expression. The first matching arm wins.
    private static string GradeFor(double mark) => mark switch
    {
        >= 80 => "HD",
        >= 70 => "D",
        >= 60 => "C",
        >= 50 => "P",
        _ => "N"
    };
}
