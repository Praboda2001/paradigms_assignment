// Student Name: P.H. Thushan Prabodha
// Student ID: 10705662
// Date: 23/09/2026

using System.ComponentModel.DataAnnotations;

// This shows the one unit a student is enrolled in.
public class UnitEnrollment
{
    // This is the Primary key, used by Entity Framework to identify this row.
    [Key]
    public int Id { get; set; }

    // This is the unit 's short code.
    [Required, MaxLength(10)]
    public string UnitCode { get; set; } = string.Empty;

    // This is the unit's full name.
    [Required]
    public string UnitName { get; set; } = string.Empty;

    // This is which semester this unit was taken in.
    public string Semester { get; set; } = string.Empty;

    // This is the grade the student received for this unit, could be letter or text.
    public string Grade { get; set; } = string.Empty;

    // This is the numeric mark the student received for this unit, used for WAM calculation.
    public double Mark { get; set; }

    // This is the Foreign key back to the owning StudentRecord.
    public int StudentRecordId { get; set; }
}