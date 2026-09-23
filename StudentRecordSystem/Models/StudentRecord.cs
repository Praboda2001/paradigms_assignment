// Student Name: P.H. Thushan Prabodha
// Student ID: 10705662
// Date: 23/09/2026

using System.ComponentModel.DataAnnotations;

namespace StudentRecordSystem.Models;

// This is a student's academic record. data plus the one operation (CalculateWAM).
public class StudentRecord
{
    // This is the Backing field for the validated Name property.
    private string _name = string.Empty;
    
    // This is the Primary key, used by Entity Framework to identify this row.
    [Key]
    public int Id { get; set;}

    // This is the Student's ID number and this must be unique.
    [Required, MaxLength(20)]
    public string StudentID { get; set; } = string.Empty;

    // This is the student's full name.
    [Required]
    public string Name
    {
        // This returns the stored name.
        get => _name;
        // This rejects an empty/whitespace-only name before storing it.
        set => _name = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Name cannot be empty.", nameof(value))
            : value;
    }

    // This is the course or program the student is enrolled in.
    public string Program { get; set; } = string.Empty;

    // This is the student's current status of enrollment: Active, Suspended or Graduated.
    public string Status { get; set; } = "Active";

    // This is the student's current Weighted Average Mark (WAM).
    public double Wam { get; set; }

    // This is the list of units the student has enrolled in, with their marks.
    public List<UnitEnrollment> Units { get; set; } = new();

    // Now we recalculates WAM using LINQ's Average instead of a manual loop.
    public double CalculateWAM()
    {
        Wam = Units.Count == 0 ? 0.0 : Math.Round(Units.Average(u => u.Mark), 2);
        return Wam;
    }

}