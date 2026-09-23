// Student Name: P.H. Thushan Prabodha
// Student ID: 10705662
// Date: 23/09/2026

namespace StudentRecordSystem.Exceptions;

// We use this when a lookup student ID fins no matching record in the system.
public class StudentNotFoundException : Exception
{
    // We use this to builds a message naming the missing ID,
    // Then passes it to the base Exception class constructor.
    public StudentNotFoundException(string studentId)
        : base($"No student record found with the ID '{studentId}'.")
    {
    }
}