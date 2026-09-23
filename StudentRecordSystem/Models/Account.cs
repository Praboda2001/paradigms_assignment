// Student Name: P.H. Thushan Prabodha
// Student ID: 10705662
// Date: 23/09/2026

using System.ComponentModel.DataAnnotations;

namespace StudentRecordSystem.Models;

// This is the two kinds of accout the system supports.
public enum AccountRole
{
    Admin,
    Student
}

// This is the persisted login credential, kept seperate from User/ Admin/ Student.
// So, authentication maps to EF Core cleanly.
public class Account
{
    // This is the Primary key, used by Entity Framework to identify this row.
    [Key]
    public int Id { get; set; }

    // This is the unique username for this account, used for login.
    [Required]
    public string Username { get; set; } = string.Empty;

    // This is the SHA-256 hash of password, never the plain-text password itself.
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    // This is the role of the account, either Admin or Student.
    public AccountRole Role { get; set; }

    // This is only populated for Student accounts, links to thier StudentRecord.
    public int? StudentRecordId { get; set; } 
}