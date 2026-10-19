// Student Name: P.H. Thushan Prabodha
// Student ID: 10705662
// Date: 23/09/2026

namespace StudentRecordSystem.Exceptions;

// We use this when an admin tries to add a student ID that already exists.
public class DuplicateStudentException : Exception
{
    // We use this to build a message naming the duplicate ID,
    // Then passes it to the base Exception class constructor.
    public DuplicateStudentException(string studentId)
        : base($"A student record with the ID '{studentId}' already exists.")
    {
    }
}
