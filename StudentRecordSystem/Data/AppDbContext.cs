// Student Name: P.H. Thushan Prabodha
// Student ID: 10705662
// Date: 23/09/2026

using Microsoft.EntityFrameworkCore;
using StudentRecordSystem.Models;

namespace StudentRecordSystem.Data;

// This is EF Core's connection to the database. 
// It's what turn our plain C# classes into SQLite tables and back again.
public class AppDbContext : DbContext
{
    // This is connection settings get passes in from MauiProgram.cs
    // rather than being hardcoded here.
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // These properties represent the tables in the database.
    public DbSet<StudentRecord> StudentRecords => Set<StudentRecord>();
    public DbSet<UnitEnrollment> UnitEnrollments => Set<UnitEnrollment>();
    public DbSet<Account> Accounts => Set<Account>();

    // EF Core calls this automatically to figure out the relationships
    // between tables that we can't just express with attributes.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // One student has many Units. So we use this if a studentRecord gets deleted,
        // its Units get deleted with it, instead of being left behind with nothing pointing to them.
        modelBuilder.Entity<StudentRecord>()
            .HasMany(r => r.Units)
            .WithOne()
            .HasForeignKey(u => u.StudentRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        // We use this to ensure no two students can share the same Student ID.
        modelBuilder.Entity<StudentRecord>()
            .HasIndex(r => r.StudentId)
            .IsUnique();

        // We use this to ensure no two accounts can share the same username.
        modelBuilder.Entity<Account>()
            .HasIndex(a => a.Username)
            .IsUnique();
    }
}
