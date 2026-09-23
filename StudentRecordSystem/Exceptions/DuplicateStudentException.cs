// Student Name: P.H. Thushan Prabodha
// Student ID: 10705662
// Date: 23/09/2026

namespace StudentRecordSystem.Exceptions;

// We use this when an admin tries to add a student ID that already exists.
public class DuplicateStudentException : Exception
{
    // We use this to builds a message naming the dulicate ID,
    // Then passes it to the base Exception class constructor.
    public DuplicateStudentException(string StudentId)
        : base($"A student record with the ID '{StudentId}' already exists.")
    {
    }
}